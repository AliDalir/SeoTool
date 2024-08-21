using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using AutoMapper;
using BusinessLayer.MongoDb;
using BusinessLayer.Repositories;
using ClosedXML.Excel;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using DataAccessLayer.MongoEntities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
using RestSharp;
using UtilityLayer.Convertors;
using UtilityLayer.GoogleSheets;
using Tag = DataAccessLayer.Entites.Tag;

namespace BusinessLayer.Services;

public class KeywordService : IKeywordService
{
    private readonly IBackgroundJobClient _backgroundJobs;

    private readonly IConfiguration _configuration;
    private readonly SeoToolDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<KeywordService> _logger;
    private readonly IHtmlService _htmlService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedCache _distributedCache;

    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMinutes(10), 10));

    private readonly IMongoCollection<SerperResponse> _serperCollection;


    public KeywordService(SeoToolDbContext context,
        IMapper mapper,
        IConfiguration configuration,
        IBackgroundJobClient backgroundJobs, ILogger<KeywordService> logger, IHtmlService htmlService,
        IOptions<MongoDbConnectionSettings> mongoDbConnectionSettings, IMongoClient client, IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _backgroundJobs = backgroundJobs;
        _logger = logger;
        _htmlService = htmlService;
        _scopeFactory = scopeFactory;
        var database = client.GetDatabase(mongoDbConnectionSettings.Value.DatabaseName);
        _serperCollection =
            database.GetCollection<SerperResponse>(mongoDbConnectionSettings.Value.SerperResponseCollectionName);
    }


    public async Task<ResponseDto> AddKeywordAsync(List<KeywordDto> keywords)
    {
        try
        {
            List<Keyword> keywordList = new List<Keyword>();

            foreach (var keyword in keywords)
            {
                if (!_context.Keywords.Any(k => k.Query == keyword.Query) &&
                    !keywordList.Any(x => x.Query == keyword.Query))
                {
                    keywordList.Add(new Keyword() { KeywordGroupId = keyword.KeywordGroupId, Query = keyword.Query });
                }
            }

            await _context.Keywords.AddRangeAsync(keywordList);
            await _context.SaveChangesAsync();

            return new ResponseDto
            {
                Error = false,
                Message = HttpStatusCode.OK.ToString()
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                Error = true,
                Message = e.Message
            };
        }
    }

    public async Task AddKeywordGroup(List<string> titles)
    {
        List<KeywordGroupReqDto> groups = new List<KeywordGroupReqDto>();

        foreach (var title in titles)
        {
            if (!_context.KeywordGroups.Any(k => k.GroupTitle == title))
            {
                KeywordGroupReqDto group = new KeywordGroupReqDto()
                {
                    GroupTitle = title
                };

                groups.Add(group);
            }
        }

        await _context.KeywordGroups.AddRangeAsync(_mapper.Map<List<KeywordGroup>>(groups));

        await _context.SaveChangesAsync();
    }

    public async Task AddSites(SiteReqDto site)
    {
        if (!_context.Sites.Any(k => k.KeywordGroupId == site.KeywordGroupId && k.SiteUrl == site.SiteUrl))
        {
            var mappedSites = _mapper.Map<Site>(site);

            await _context.Sites.AddAsync(mappedSites);

            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateTrackRequest(int? catId)
    {
        if (catId == null)
        {
            var cats = await _context.KeywordGroups.ToListAsync();

            var count = cats.Count;
            var index = 0;

            foreach (var cat in cats)
            {
                index++;
                _backgroundJobs.Schedule(() => TrackKeywordsRankAsync(cat.Id),
                    DateTimeOffset.Now.AddMinutes((count * 15) - (index * 15)));
            }
        }
        else if (catId != null)
        {
            _backgroundJobs.Schedule(() => TrackKeywordsRankAsync(catId), DateTimeOffset.Now.AddMinutes(1));
        }
    }

    public async Task<ResponseDto> TrackKeywordsRankAsync(int? catId)
    {
        try
        {
            _logger.LogWarning("Tracking keyword is start for catId: `{@cId}` at {@time}",
                catId == null ? "All the cats" : catId, DateTime.Now);

            var crawlDates = await _context.CrawlDates
                .ToListAsync();

            var crawlDate = crawlDates.Where(d =>
                d.CrawlDateTime.Date.ToShortDateString() == DateTime.Now.Date.ToShortDateString()).FirstOrDefault();


            if (crawlDate == null)
            {
                _logger.LogWarning("No other crawl request today");

                crawlDate = new CrawlDate
                {
                    CrawlDateTime = DateTime.Now
                };

                await _context.CrawlDates.AddAsync(crawlDate);

                await _context.SaveChangesAsync();
            }
            


            var keywordGroups = await _context.KeywordGroups.Where(kg => kg.Id == catId).ToListAsync();

            _logger.LogWarning("{@count} keyword groups found", keywordGroups.Count);
            

            foreach (var keywordGroup in keywordGroups)
            {
                _logger.LogWarning("Foreach on keyword group. Group Is   '{@Group}'   with Id: {@GroupId}",
                    keywordGroup.GroupTitle, keywordGroup.Id);

                var sites = await _context.Sites
                    .Where(s => s.KeywordGroupId == keywordGroup.Id)
                    .ToListAsync();

                _logger.LogWarning("{@count}   sites groups found for  '{@Group}'  Group", keywordGroup.GroupTitle,
                    sites.Count);
                
                

                var keywords = _context.Keywords
                    .Where(k => k.KeywordGroupId == keywordGroup.Id)
                    .ToList();

                _logger.LogWarning("{@count} keywords found for  '{@Group}'  Group", keywordGroup.GroupTitle,
                    keywordGroups.Count);


                // Assuming 'keywords' is a List<Keyword> and already populated
                int batchSize = 100;
                
                var existing = _serperCollection
                    .Find(s => s.CrawlDateId ==
                        crawlDate.Id && s.KeywordGroupId == catId).ToList();
                
                int idCounter = existing.Count == 0 ? 0 : existing.Count;

                if (existing.Count > 0)
                {
                    for (int b = (existing.Count * 100); b < keywords.Count; b += batchSize)
                    {
                        idCounter++;
                        var batchQueries = keywords.Skip(b).Take(batchSize).Select(k => k.Query).ToList();
                        var response = await RequestKeywordRank(batchQueries); // This starts the task
                        await SaveSeperResponseToMongo(response,(crawlDate.Id + "-" + keywordGroup.Id + "-" + idCounter).ToString(),crawlDate.Id,keywordGroup.Id);
                    }
                }
                else if (existing == null || existing.Count == 0)
                {
                    for (int b = 0; b < keywords.Count; b += batchSize)
                    {
                        idCounter++;
                        var batchQueries = keywords.Skip(b).Take(batchSize).Select(k => k.Query).ToList();
                        var response = await RequestKeywordRank(batchQueries); // This starts the task
                        await SaveSeperResponseToMongo(response,(crawlDate.Id + "-" + keywordGroup.Id + "-" + idCounter).ToString(),crawlDate.Id,keywordGroup.Id);
                    }
                }
                
                
                await ProccessKeywordTracktion(keywordGroup.Id, crawlDate.Id, keywords, sites);
            }


            var keywordCount = await _context.Keywords.Where(kg => kg.KeywordGroupId == catId).CountAsync();

            await GetRanksByCategoryForExcelExportAsync(catId??0, keywordCount, 1, null, null);

            await RequestGenerateKeywordGroupHistory(catId??0, crawlDate.Id);
            
            _logger.LogWarning("Cache with the key :'AllRanks' removed successfully");

            return new ResponseDto
            {
                Error = false,
                Message = $"succesfully update"
            };
        }
        catch (Exception e)
        {
            if (e is ValidationException validationException)
            {
                _logger.LogError("Exception occurred:  {@Message}  {@Errors} {@Exception}", e.Message,
                    validationException.Message, validationException);
            }

            _logger.LogError("Error in tracking keywords. Error is : {@error}", e.Message);

            return new ResponseDto
            {
                Error = true,
                Message = e.Message
            };
        }
    }

    public async Task SaveSeperResponseToMongo(List<SerpApiDto> response, string id, int crawlDateId, int KeywordGroupId)
    {
        SerperResponse serper = new SerperResponse()
        {
            Responses = response,
            Id = id,
            CrawlDateId = crawlDateId,
            KeywordGroupId = KeywordGroupId
        };
        var check =  _serperCollection.Find(x => x.Id == id).FirstOrDefault();

        if (check == null)
        {
            await _serperCollection.InsertOneAsync(serper);
        }
        else
        {
            await _serperCollection
                .ReplaceOneAsync(x => x.Id == id, serper);
        }
    }

    public async Task ProccessKeywordTracktion(int catId, int crawlDateId, List<Keyword> keywords, List<Site> sites)
    {
        var response = _serperCollection.Find(s => s.CrawlDateId == crawlDateId && s.KeywordGroupId == catId).ToList();

        foreach (var data in response)
        {
            foreach (var res in data.Responses)
            {
                var keyword = keywords.FirstOrDefault(x => x.Query == res.SearchParameters.q);
                if (keyword == null || res.Organic.Count == 0)
                {
                    _logger.LogWarning("There is no result for {@keyword}", keyword?.Query);
                    continue; // Skip to the next iteration if no keyword or no organic results
                }

                _logger.LogWarning("{@count} results found for {@keyword}", res.Organic.Count, keyword.Query);

                foreach (var site in sites)
                {
                    ; // Process each site in an async method
                    await ProcessSiteRankAsync(site, res, keyword,
                        crawlDateId); // Add the task to the list without awaiting here
                }
            }
        }
        
        
    }

    public async Task ProcessSiteRankAsync(Site site, SerpApiDto res, Keyword keyword, int crawlDateId)
    {
        // Assuming ResponseType is the type of res
        // Logic to process each site rank, similar to your existing foreach loop for sites
        // Replace the content of this method with the site processing logic, including calls to AddKeywordUrl and AddRank

        BaseRankDto rank = new BaseRankDto();
        // Populate rank based on logic

        if (res.AnswerBox?.Link?.Contains(site.SiteUrl) == true)
        {
            // Logic for AnswerBox
            
            rank = new BaseRankDto
            {
                KeywordId = keyword.Id,
                Location = "fa_ir",
                Position = 0,
                SiteId = site.Id,
                CrawlDateId = crawlDateId
            };
            
            await AddKeywordUrl(res.AnswerBox.Link, keyword.Id, crawlDateId);
            // Update rank as necessary
        }

        var organicResult = res.Organic.FirstOrDefault(r => r.Link.Contains(site.SiteUrl));
        
        if (organicResult != null)
        {
            // Logic for organic results
            rank = new BaseRankDto
            {
                KeywordId = keyword.Id,
                Location = "fa_ir",
                Position = organicResult != null ? organicResult.Position : 101,
                SiteId = site.Id,
                CrawlDateId = crawlDateId
            };
            await AddKeywordUrl(organicResult.Link, keyword.Id, crawlDateId);
            // Update rank as necessary
        }

        // Set rank if not already set
        if (rank.Location == null)
        {
            // Default rank logic
            rank = new BaseRankDto
            {
                KeywordId = keyword.Id,
                Location = "fa_ir",
                Position = 101,
                SiteId = site.Id,
                CrawlDateId = crawlDateId
            };
        }

        await AddRank(rank);
        _logger.LogWarning("Rank For '{@keyword}' added successfully", keyword.Query);
    }

    public async Task<List<SerpApiDto>> RequestKeywordRank(List<string> queries)
    {
        try
        {
            var options = new RestClientOptions("https://google.serper.dev/search")
            {
                ThrowOnAnyError = true,
                MaxTimeout = 60000
            };
            var client = new RestClient(options);

            var request = new RestRequest()
            {
                Method = Method.Post
            };

            request.AddHeader("X-API-KEY", "733e06cff1c6d08c32200c6213a6fa442cca5ceb");

            request.AddHeader("Content-Type", "application/json");

            var body = new List<SerperBody>();

            foreach (var query in queries)
            {
                body.Add(new SerperBody { q = query, gl = "ir", hl = "fa", num = 100 });
            }

            var jsonBody = JsonConvert.SerializeObject(body);

            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = client.Execute(request);


            var results = JsonConvert.DeserializeObject<List<SerpApiDto>>(response.Content);

            return results;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


    public async Task<List<NightWatchDto>> GetAllKeywordsFromNightWatch()
    {
        var baseUrl = "https://api.nightwatch.io/";
        var apiKey = "215e860b16113f8e591f087cb2922eb4eb3a46f34a427d96a1579661408e3050";
        var urlId = "165625";


        var page = 1;

        var keywordList = new List<NightWatchDto>();

        while (true)
            // Make a request to the Nightwatch API for the current page
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(
                           $"https://api.nightwatch.io/api/v1/urls/{urlId}/keywords?access_token={apiKey}&page={page}&limit=500"))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<NightWatchDto>>(apiResponse);
                    keywordList.AddRange(data);
                    page++;

                    if (data.Count == 0) break;
                }
            }

        return keywordList;
    }

    public async Task AddRank(BaseRankDto rank)
    {
        var mappedRank = _mapper.Map<Rank>(rank);

        mappedRank.CreationDateTime = DateTime.Now;
        mappedRank.ModificationDateTime = mappedRank.CreationDateTime;

        _context.Add(mappedRank);
        await _context.SaveChangesAsync();
    }


    public async Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsync(int catId, int pageSize, int pageIndex,
        string? rank, int? tagId)
    {
        try
        {
            var sites = await FindSitesForHistoricalData(catId);

            var keywords = await FindKeywordsForHistoricalData(catId, pageSize, pageIndex, rank, tagId);

            var query = await GenerateQueryDto(keywords.Item1, catId);
            

            var keywordGroupHistories = await _context.KeywordGroupHistories
                .Where(h => h.KeywordGroupId == catId)
                .Include(h => h.CrawlDate)
                .ToListAsync();

            var rankDto = new RankDto
            {
                KeywordGroupHistory = _mapper.Map<List<KeywordGroupHistoryDto>>(keywordGroupHistories),
                Sites = _mapper.Map<List<SiteDto>>(sites),
                // Queries = query
            };

            var response = new PaginitionResDto<RankDto>
            {
                Data = rankDto,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = keywords.Item2
            };

            return response;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public async Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsyncOptimized(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
        try
        {
            // var sitesTask = FindSitesForHistoricalData(catId);
            var keywordsTask = FindKeywordsForHistoricalDataOptimized(catId, pageSize, pageIndex, rank, tagId);

            // await Task.WhenAll(sitesTask, keywordsTask);
            //
            // var sites = await sitesTask;
            var keywords = await keywordsTask;

            var keywordGroupHistories = await _context.KeywordGroupHistories
                .Where(h => h.KeywordGroupId == catId)
                .Include(h => h.CrawlDate)
                .ToListAsync();

            var rankDto = new RankDto
            {
                KeywordGroupHistory = _mapper.Map<List<KeywordGroupHistoryDto>>(keywordGroupHistories),
                // Sites = _mapper.Map<List<SiteDto>>(sites),
                Queries = keywords.Item1
            };

            var response = new PaginitionResDto<RankDto>
            {
                Data = rankDto,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = keywords.Item2
            };


            return response;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public async Task<string> GetRanksByCategoryForExcelExportAsync(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
        try
        {
            var sites = await FindSitesForHistoricalData(catId);
            
            _logger.LogWarning("Site Found {@count}", sites.Count);

            var keywords = await FindKeywordsForHistoricalData(catId, pageSize, pageIndex, rank, tagId);
            
            _logger.LogWarning("Keywords Found {@count}", keywords.Item1.Count);

            var query = await GenerateQueryDtoForExcelExport(keywords.Item1, catId);
            
            _logger.LogWarning("query generated");

            var rankDto = new RankDto
            {
                Sites = _mapper.Map<List<SiteDto>>(sites),
                // Queries = query
            };

            var response = new PaginitionResDto<RankDto>
            {
                Data = rankDto,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = keywords.Item2
            };

            
            _logger.LogWarning("going to save excel file");
            var fileName= await ExportKeywordsToExcel(rankDto,catId);

            _logger.LogWarning("filename is : {@filename}",fileName);
            
            return fileName;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public async Task<PaginitionResDto<RankDto>> GenerateHistoricalDataByCatIdAsync(int catId, int pageSize,
        int pageIndex)
    {
        try
        {
            var sites = new List<Site>();

            var keywords = new List<Keyword>();

            if (catId == 4)
            {
                sites = await _context.Sites
                    .Where(site => site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()))
                    .ToListAsync();

                keywords = await _context.Keywords
                    .Where(k => k.KeywordGroup.Sites
                        .Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())))
                    .Include(s => s.KeywordUrls)
                    .Include(s => s.KeywordTags)
                    .ThenInclude(s => s.Tag)
                    .Include(k => k.Ranks
                        .Where(r => r.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())))
                    .ThenInclude(r => r.Site)
                    .ToListAsync();
            }
            else
            {
                sites = await _context.Sites
                    .Where(s => s.KeywordGroupId == catId)
                    .ToListAsync();

                
                _logger.LogWarning("Site Found {@count}", sites.Count);

                keywords = await _context.Keywords
                    .Where(k => k.KeywordGroupId == catId)
                    .Include(s => s.KeywordUrls)
                    .Include(s => s.KeywordTags)
                    .ThenInclude(s => s.Tag)
                    .Include(k => k.Ranks)
                    .ThenInclude(k => k.Site)
                    .ToListAsync();
                
                _logger.LogWarning("Keyword Found {@Kcount}", keywords.Count);
            }


            var query = keywords.Select(keyword => new QueryDto
            {
                KeywordId = keyword.Id,
                Query = keyword.Query,
                AvgPosition = keyword.Ranks
                    .Where(r => r.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()))
                    .Select(r => r.Position)
                    .DefaultIfEmpty() // Ensures that an empty sequence results in a default value (0.0 for double)
                    .Average()
                    .ToString("0.00"),
                CurrentPosition = keyword.Ranks
                    .Where(r => r.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()))
                    .OrderByDescending(r => r.CreationDateTime)
                    .FirstOrDefault()?.Position.ToString(),
                LastTrackTime = keyword.Ranks
                    .Where(r => r.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()))
                    .OrderByDescending(r => r.CreationDateTime)
                    .FirstOrDefault()?.CreationDateTime.ToString("yyyy/MM/dd"),
                Ranks = catId == 4
                    ? null
                    : _mapper.Map<List<BaseRankDto>>(
                        keyword.Ranks.GroupBy(r => r.SiteId)
                            .Select(group => group.OrderByDescending(r => r.CreationDateTime).FirstOrDefault())
                            .ToList()
                    ),
                KeywordUrls = _mapper.Map<List<KeywordUrlDto>>(keyword.KeywordUrls),
                Tags = _mapper.Map<List<Tag>>(keyword.KeywordTags.Select(x => new Tag()
                {
                    TagName = x.Tag.TagName,
                    Id = x.Tag.Id
                }))
            }).ToList();
            
            _logger.LogWarning("Qouery Count {@qount}", query.Count);


            var crawlDates = await _context.CrawlDates.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            
            var keywordGroupHistory = await _context.KeywordGroupHistories
                .Where(kgh => kgh.KeywordGroupId == catId && kgh.CrawlDateId == crawlDates.Id).FirstOrDefaultAsync();

            if (keywordGroupHistory == null)
                keywordGroupHistory = await GenerateKeywordGroupHistory(query, crawlDates.Id, catId);

            
            _logger.LogWarning("keyword group generated");
            
            var keywordGroupHistories = await _context.KeywordGroupHistories
                .Where(h => h.KeywordGroupId == catId)
                .Include(h => h.CrawlDate)
                .ToListAsync();

            var rank = new RankDto
            {
                KeywordGroupHistory = _mapper.Map<List<KeywordGroupHistoryDto>>(keywordGroupHistories),
                Sites = _mapper.Map<List<SiteDto>>(sites),
                // Queries = query
            };

            var response = new PaginitionResDto<RankDto>
            {
                Data = rank,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = query.Count
            };

            // _cache.Set("RankCatId:" + catId, response, DateTimeOffset.Now.AddHours(1));
            

            return response;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public async Task<List<QueryDto>> GenerateQueryDto(List<Keyword> keywords, int catId)
    {
        var query = keywords.Select(keyword => new QueryDto
        {
            KeywordId = keyword.Id,
            Query = keyword.Query,
            AvgPosition = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .Select(r => r.Position)
                .DefaultIfEmpty() // Ensures that an empty sequence results in a default value (0.0 for double)
                .Average()
                .ToString("0.00"),
            CurrentPosition = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .OrderByDescending(r => r.CreationDateTime)
                .FirstOrDefault()?.Position.ToString(),
            LastTrackTime = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .OrderByDescending(r => r.CreationDateTime)
                .FirstOrDefault()?.CreationDateTime.ToString("yyyy/MM/dd"),
            SearchVolume = string.IsNullOrEmpty(keyword.KeywordSearchVolume?.SearchVolume)
                ? "0"
                : keyword.KeywordSearchVolume.SearchVolume,
            Ranks = catId == 4
                ? null
                : _mapper.Map<List<BaseRankDto>>(
                    keyword.Ranks.GroupBy(r => r.SiteId)
                        .Select(group => group.OrderByDescending(r => r.CreationDateTime).FirstOrDefault())
                        .ToList()
                ),
            KeywordUrls = _mapper.Map<List<KeywordUrlDto>>(keyword.KeywordUrls),
            Tags = _mapper.Map<List<Tag>>(keyword.KeywordTags.Select(x => new Tag()
            {
                TagName = x.Tag?.TagName,
                Id = x.Tag?.Id ?? 0 // Assuming default ID is 0 if Tag is null
            }))
        }).ToList();

        return query;

    }

    public async Task<List<QueryDto>> GenerateQueryDtoForExcelExport(List<Keyword> keywords, int catId)
    {
        var query = keywords.Select(keyword => new QueryDto
        {
            KeywordId = keyword.Id,
            Query = keyword.Query,
            AvgPosition = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .Select(r => r.Position)
                .DefaultIfEmpty() // Ensures that an empty sequence results in a default value (0.0 for double)
                .Average()
                .ToString("0.00"),
            CurrentPosition = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .OrderByDescending(r => r.CreationDateTime)
                .FirstOrDefault()?.Position.ToString(),
            LastTrackTime = keyword.Ranks
                .Where(r => r.Site?.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()) == true)
                .OrderByDescending(r => r.CreationDateTime)
                .FirstOrDefault()?.CreationDateTime.ToString("yyyy/MM/dd"),
            SearchVolume = string.IsNullOrEmpty(keyword.KeywordSearchVolume?.SearchVolume)
                ? "0"
                : keyword.KeywordSearchVolume.SearchVolume,
            Ranks = catId == 4
                ? null
                : _mapper.Map<List<BaseRankDto>>(
                    keyword.Ranks.GroupBy(r => r.SiteId)
                        .Select(group => group.OrderByDescending(r => r.CreationDateTime).FirstOrDefault())
                        .ToList()
                )
        }).ToList();

        return query;
    }


    public async Task<Tuple<List<Keyword>,int>> FindKeywordsForHistoricalData(int catId, int pageSize, int pageIndex, string? rank,
        int? tagId)
    {
        var keywords = new List<Keyword>();
        var queryableKeywords = _context.Keywords.AsQueryable();
        int resPonseCount = 0; 

        if (catId == 4)
        {
            if (rank != null)
            {
                switch (rank)
                {
                    case "Top3":

                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordGroup.Sites.Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())) &&
                                        k.Ranks
                                            .OrderByDescending(r => r.CreationDateTime)
                                            .FirstOrDefault()
                                            .Position < 4);

                        break;
                    case "Top10":

                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordGroup.Sites.Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())) &&
                                        k.Ranks
                                            .OrderByDescending(r => r.CreationDateTime)
                                            .FirstOrDefault()
                                            .Position >= 4 &&
                                        k.Ranks.OrderByDescending(r => r.CreationDateTime).FirstOrDefault().Position <=
                                        10);
                        break;

                    case "Top100":

                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordGroup.Sites.Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())) &&
                                        k.Ranks
                                            .OrderByDescending(r => r.CreationDateTime)
                                            .FirstOrDefault()
                                            .Position > 10 &&
                                        k.Ranks.OrderByDescending(r => r.CreationDateTime).FirstOrDefault().Position <=
                                        100);


                        break;

                    case "No-Rank":

                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordGroup.Sites.Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())) &&
                                        k.Ranks
                                            .OrderByDescending(r => r.CreationDateTime)
                                            .FirstOrDefault()
                                            .Position > 100);
                        break;
                }
            }

                if (tagId.HasValue)
                {
                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
                }

                resPonseCount = queryableKeywords.Count();


                keywords = await queryableKeywords
                    .OrderByDescending(x => Convert.ToInt32(x.KeywordSearchVolume.SearchVolume))
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Include(s => s.KeywordUrls)
                    .Include(sv => sv.KeywordSearchVolume)
                    .Include(s => s.KeywordTags)
                    .ThenInclude(s => s.Tag)
                    .Include(k => k.Ranks
                        .Where(r => r.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())))
                    .ThenInclude(r => r.Site)
                    .ToListAsync();
            
        }
        else
        {
            switch (rank)
            {
                case "Top3":

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == catId &&
                                    k.Ranks
                                        .OrderByDescending(r => r.CreationDateTime)
                                        .FirstOrDefault()
                                        .Position < 4);

                    break;
                case "Top10":

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == catId &&
                                    k.Ranks
                                        .OrderByDescending(r => r.CreationDateTime)
                                        .FirstOrDefault()
                                        .Position >= 4 &&
                                    k.Ranks.OrderByDescending(r => r.CreationDateTime).FirstOrDefault().Position <= 10);
                    break;

                case "Top100":

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == catId &&
                                    k.Ranks
                                        .OrderByDescending(r => r.CreationDateTime)
                                        .FirstOrDefault()
                                        .Position > 10 &&
                                    k.Ranks.OrderByDescending(r => r.CreationDateTime).FirstOrDefault().Position <=
                                    100);


                    break;

                case "No-Rank":

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == catId &&
                                    k.Ranks
                                        .OrderByDescending(r => r.CreationDateTime)
                                        .FirstOrDefault()
                                        .Position > 100);
                    break;
            }
            
            if (tagId.HasValue)
            {
                queryableKeywords = queryableKeywords
                    .Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
            }

            if (rank == null)
            {
                queryableKeywords = queryableKeywords
                    .Where(k => k.KeywordGroupId == catId &&
                                k.Ranks
                                    .OrderByDescending(r => r.CreationDateTime)
                                    .FirstOrDefault()
                                    .Position < 200);
            }
        
            
            resPonseCount = queryableKeywords.Count();


            if (pageSize > 20)
            {
                for (int i = 0; i < pageSize; i = i + 20)
                {
                    
                    var listOfKeywords = await queryableKeywords
                        .Where(k => k.KeywordGroupId == catId)
                        .Skip(i)
                        .Include(sv => sv.KeywordSearchVolume)
                        .Include(k => k.Ranks)
                        .ThenInclude(k => k.Site)
                        .AsNoTracking()
                        .Take(20)
                        .ToListAsync();
                    
                    keywords.AddRange(listOfKeywords);
                }
            }
            else
            {
                keywords = await queryableKeywords
                    .Where(k => k.KeywordGroupId == catId)
                    .OrderByDescending(x => Convert.ToInt32(x.KeywordSearchVolume.SearchVolume))
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Include(s => s.KeywordUrls)
                    .Include(sv => sv.KeywordSearchVolume)
                    .Include(s => s.KeywordTags)
                    .ThenInclude(s => s.Tag)
                    .Include(k => k.Ranks)
                    .ThenInclude(k => k.Site)
                    .AsNoTracking()
                    .ToListAsync();
            }

        }


        return Tuple.Create(keywords, resPonseCount);
    }

    public async Task<Tuple<List<KeywordResponseDto>, int>> FindKeywordsForHistoricalDataOptimized(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
        var queryableKeywords = _context.Keywords.AsQueryable()
            .Where(k => catId == 4 ? k.KeywordGroup.Sites.Any(s => s.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())) : k.KeywordGroupId == catId);
        
        
        if (!string.IsNullOrEmpty(rank))
        {
            int lowerBound = 0, upperBound = int.MaxValue;
            switch (rank)
            {
                case "Top3":
                    upperBound = 3;
                    break;
                case "Top10":
                    lowerBound = 4;
                    upperBound = 10;
                    break;
                case "Top100":
                    lowerBound = 11;
                    upperBound = 100;
                    break;
                case "No-Rank":
                    lowerBound = 100;
                    upperBound = 200;
                    break;
            }

            queryableKeywords = queryableKeywords
                .Where(k => k.Ranks.Any(r => r.Position >= lowerBound && r.Position <= upperBound && 
                                             r.CrawlDateId == k.Ranks.Max(rm => rm.CrawlDateId)));
        }

        
        if (tagId.HasValue)
        {
            queryableKeywords = queryableKeywords.Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
        }
        
        var totalCount = await queryableKeywords.CountAsync();
        
        
        var keywords = queryableKeywords
            .Select(k => new
            {
                k.KeywordGroupId,
                k.Query,
                k.Id,
                CurrentRankPosition = k.Ranks.Where(u => u.Site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString())).Select(r => (int?)r.Position).FirstOrDefault(),
                URL = k.KeywordUrls.Select(u => u.Url).FirstOrDefault(),
                KeywordTags = k.KeywordTags.Select(t => new TagReqDto(){TagName = t.Tag.TagName,Id = t.TagId}), // Assuming you want tag names; adjust as necessary
                SearchVolume = k.KeywordSearchVolume.SearchVolume,
                // Example of how you might handle CompetitorsRank if you need summary info; adjust according to actual requirements
            })
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize);

        var projectedKeywords = keywords.Select(k => new KeywordResponseDto
        {
            KeywordGroupId = k.KeywordGroupId,
            KeywordId = k.Id,
            Query = k.Query,
            CurrentRank = k.CurrentRankPosition ?? 0,
            URL = k.URL ?? "",
            KeywordTags = k.KeywordTags.ToList(), // Adjust based on your DTO's structure
            KeywordSearchVolume = k.SearchVolume ?? "0",
        }).ToList();
        
        

        return new Tuple<List<KeywordResponseDto>, int>(projectedKeywords, totalCount);

    }

    public async Task<List<Site>> FindSitesForHistoricalData(int catId)
    {

        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SeoToolDbContext>();
            
            
            var sites = new List<Site>();


            if (catId == 4)
            {
                sites = await dbContext.Sites
                    .Where(site => site.SiteUrl.Contains(_configuration.GetSection("DomainName").ToString()))
                    .ToListAsync();
            }
            else
            {
                sites = await dbContext.Sites
                    .Where(s => s.KeywordGroupId == catId)
                    .ToListAsync();
            }


            return sites;
        }
        
        
    }

    public async Task<PaginitionResDto<List<KeywordGroupDto>>> GetAllKeywordGroupsAsync()
    {
        // _backgroundJobs.Enqueue(() => Console.WriteLine("Hello world hang fire"));


        var groups = await _context.KeywordGroups.ToListAsync();

        var mappedGroups = _mapper.Map<List<KeywordGroupDto>>(groups);


        var response = new PaginitionResDto<List<KeywordGroupDto>>
        {
            Data = mappedGroups.OrderByDescending(mg => mg.Id).ToList(),
            TotalCount = groups.Count
        };

        return response;
    }

    public async Task RequestGenerateKeywordGroupHistory(int catId,int crawlDateId)
    {
        // var sites = await FindSitesForHistoricalData(catId);
        //
        // var keywords = await FindKeywordsForHistoricalData(catId, 200000, 1, null, null);
        //
        // var query = await GenerateQueryDto(keywords, catId);

        await GenerateHistoricalDataByCatIdAsync(catId,100,1);
    }

    public async Task<KeywordGroupHistory> GenerateKeywordGroupHistory(List<QueryDto> query, int crawlDateId,
        int groupId)
    {
        

        var crawlDates = await _context.CrawlDates.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
        
        
        if (query.Count > 0)
        {
            var noRankKeywords =
                query.Where(q => Convert.ToInt32(q.CurrentPosition) > 100).ToList();

            var rankedKeywords = query.Except(noRankKeywords).ToList();

            var top100Keywords = rankedKeywords.Where(q =>
                Convert.ToDouble(q.CurrentPosition) >= 10 && Convert.ToDouble(q.CurrentPosition) < 100).ToList();


            var top10Keywords = rankedKeywords.Where(q =>
                Convert.ToDouble(q.CurrentPosition) >= 4 && Convert.ToDouble(q.CurrentPosition) < 10).ToList();

            var top3Keywords = rankedKeywords.Where(q => Convert.ToDouble(q.CurrentPosition) < 4).ToList();

            var keywordGroupHistory = new KeywordGroupHistory
            {
                KeywordGroupId = groupId,

                KeywordCount = query.Count,

                NoRankCount = noRankKeywords.Count,

                AllAvgPosition = Math.Round(rankedKeywords
                    .Sum(q => Convert.ToDouble(q.AvgPosition)) / rankedKeywords.Count, 2),

                LastAvgPosition = rankedKeywords
                    .Sum(q => Convert.ToDouble(q.AvgPosition)) / rankedKeywords.Count,

                Top3Count = top3Keywords.Count,

                Top3AvgPosition = top3Keywords.Count > 0
                    ? top3Keywords.Sum(q => Convert.ToDouble(q.CurrentPosition)) / top3Keywords.Count
                    : 0,

                Top10Count = top10Keywords.Count,

                Top10AvgPosition = top10Keywords.Count > 0
                    ? top10Keywords.Sum(q => Convert.ToDouble(q.CurrentPosition)) / top10Keywords.Count
                    : 0,

                Top100Count = top100Keywords.Count,

                Top100AvgPosition = top100Keywords.Count > 0
                    ? top100Keywords.Sum(q => Convert.ToDouble(q.CurrentPosition)) / top100Keywords.Count
                    : 0,
                CreationDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                CrawlDateId = crawlDateId
            };

            await _context.KeywordGroupHistories.AddAsync(keywordGroupHistory);
            await _context.SaveChangesAsync(true);

            return keywordGroupHistory;
        }

        else
        {
            var keywordGroupHistory = new KeywordGroupHistory
            {
                KeywordGroupId = groupId,

                KeywordCount = query.Count,

                NoRankCount = 0,

                AllAvgPosition = 0,

                LastAvgPosition = 0,

                Top3Count = 0,

                Top3AvgPosition = 00,

                Top10Count = 0,

                Top10AvgPosition = 0,

                Top100Count = 0,

                Top100AvgPosition = 0,
                CreationDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now,
                CrawlDateId = crawlDateId
            };

            return keywordGroupHistory;
        }
    }

    public async Task<SingleResDto<List<SiteDto>>> GetKeywordSites(int keywordId)
    {
        // var cache = _cache.Get<SingleResDto<List<SiteDto>>>($"keywordSite:{keywordId}");
        //
        // if (cache != null) return cache;

        var keyword = await _context.Keywords
            .Where(k => k.Id == keywordId)
            .FirstOrDefaultAsync();

        var sites = await _context.KeywordGroups.Where(kg => kg.Id == keyword.KeywordGroupId)
            .Select(x => x.Sites)
            .ToListAsync();

        var mappedSites = _mapper.Map<List<SiteDto>>(sites.First());

        var response = new SingleResDto<List<SiteDto>>
        {
            Data = mappedSites
        };


        // _cache.Set($"keywordSite:{keywordId}", response, DateTimeOffset.Now.AddHours(1));


        return response;
    }

    public async Task<SingleResDto<KeywordHistoryDto>> GetKeywordDetailAsync(int id)
    {
        var keyword = await _context.Keywords
            .Where(k => k.Id == id)
            .Include(k => k.KeywordUrls)
            .Include(k => k.KeywordGroup)
            .Include(k => k.KeywordGroup.Sites)
            .Include(k => k.Ranks)
            .FirstOrDefaultAsync();

        var sites = await _context.KeywordGroups.Where(kg => kg.Id == keyword.KeywordGroupId)
            .Select(x => x.Sites)
            .ToListAsync();

        var ranks = new List<Rank>();


        foreach (var site in sites.FirstOrDefault())
        {
            var siteRanks = await _context.Ranks
                .Where(x => x.SiteId == site.Id && x.KeywordId == keyword.Id)
                .GroupBy(x => x.CrawlDate.CrawlDateTime.Date)
                .Select(group => group.OrderBy(rank => rank.CreationDateTime).First())
                .ToListAsync();

            ranks.AddRange(siteRanks);
        }


        var mappedRanks = _mapper.Map<List<BaseRankDto>>(ranks);

        var keywordDto = new KeywordHistoryDto
        {
            Sites = _mapper.Map<List<SiteDto>>(sites.First()),
            KeywordGroupId = keyword.KeywordGroupId,
            KeywordGroup = _mapper.Map<KeywordGroupDto>(keyword.KeywordGroup),
            Query = keyword.Query,
            Ranks = mappedRanks,
            KeywordUrls = _mapper.Map<List<KeywordUrlDto>>(keyword.KeywordUrls)
        };
        var response = new SingleResDto<KeywordHistoryDto>
        {
            Data = keywordDto
        };

        return response;
    }

    public async Task<SingleResDto<KeywordDto>> GetKeywordAsync(int id)
    {
        var keyword = await _context.Keywords.Where(k => k.Id == id).FirstOrDefaultAsync();

        var response = new SingleResDto<KeywordDto>
        {
            Data = _mapper.Map<KeywordDto>(keyword)
        };

        return response;
    }

    public async Task<SingleResDto<List<KeywordDto>>> GetAllKeywordsAsync()
    {
        var keywords = await _context.Keywords.ToListAsync();

        var response = new SingleResDto<List<KeywordDto>>
        {
            Data = _mapper.Map<List<KeywordDto>>(keywords),
        };

        return response;
    }

    public async Task AddVerticalsWithCompetitors(List<VerticalWithCompetitorsDto> verticalWithCompetitors)
    {
        List<string> verticals = new List<string>();


        foreach (var vertical in verticalWithCompetitors)
        {
            if (!verticals.Any(v => v == vertical.VerticalTitle))
            {
                verticals.Add(vertical.VerticalTitle);
            }
        }

        await AddKeywordGroup(verticals);


        List<SiteReqDto> sites = new List<SiteReqDto>();

        foreach (var competitor in verticalWithCompetitors)
        {
            var groupId = _context.KeywordGroups.Where(g => g.GroupTitle == competitor.VerticalTitle)
                .FirstOrDefault().Id;

            if (!sites.Any(s => s.SiteUrl == competitor.CompetitorUrl))
            {
                var urlIndex = competitor.CompetitorUrl.IndexOf(".");

                var site = new SiteReqDto()
                {
                    KeywordGroupId = groupId,
                    SiteName = competitor.CompetitorUrl.Substring(0, urlIndex),
                    SiteUrl = competitor.CompetitorUrl
                };

                await AddSites(site);
            }
        }
    }

    public async Task AddTag(List<TagDto> tags)
    {
        List<Tag> tagList = new List<Tag>();
        foreach (var tag in tags)
        {
            if (!await _context.Tags.AnyAsync(t => t.TagName == tag.TagName))
            {
                tagList.Add(new Tag() { TagName = tag.TagName });
            }
        }

        await _context.Tags.AddRangeAsync(tagList);
        await _context.SaveChangesAsync();
    }

    public async Task AddKeywordsWithTags(List<KeywordWithTagsDto> keywordWithTags)
    {
        List<TagDto> tags = new List<TagDto>();

        foreach (var tag in keywordWithTags)
        {
            if (!tags.Any(t => t.TagName == tag.TagTitle))
            {
                tags.Add(new TagDto() { TagName = tag.TagTitle });
            }
        }

        await AddTag(tags);

        List<KeywordDto> keywords = new List<KeywordDto>();

        foreach (var keyword in keywordWithTags)
        {
            if (!keywords.Any(k => k.Query == keyword.Keyword))
            {
                var group = await _context.KeywordGroups
                    .Where(g => g.GroupTitle == keyword.KeywordGroupTitle)
                    .FirstOrDefaultAsync();

                if (group != null)
                {
                    keywords.Add(new KeywordDto() { Query = keyword.Keyword, KeywordGroupId = group.Id });
                }
            }
        }

        var distinctKeywords = keywords.DistinctBy(x => x.Query).ToList();

        await AddKeywordAsync(distinctKeywords);

        var allTags = await _context.Tags.ToListAsync();

        foreach (var item in keywordWithTags)
        {
            await AddTagToKeyword(new KeywordTag()
            {
                TagId = allTags.Where(t => t.TagName == item.TagTitle).FirstOrDefault().Id,
                KeywordId = _context.Keywords.Where(k => k.Query == item.Keyword).FirstOrDefault().Id
            });
        }
    }

    public async Task AddTagToKeyword(KeywordTag keywordTag)
    {
        if (!_context.KeywordTags.Any(k => k.KeywordId == keywordTag.KeywordId && k.TagId == keywordTag.TagId))
        {
            await _context.AddAsync(keywordTag);
            await _context.SaveChangesAsync();

            var catId = await _context.Keywords.FindAsync(keywordTag.KeywordId);
        }
    }

    public async Task<SingleResDto<List<TagReqDto>>> GetAllTags()
    {
        var tags = await _context.Tags.ToListAsync();


        var response = new SingleResDto<List<TagReqDto>>()
        {
            Data = _mapper.Map<List<TagReqDto>>(tags),
        };

        return response;
    }

    public async Task<SingleResDto<List<TagReqDto>>> GetKeywordTagById(int keywordId)
    {
        var tags = await _context.KeywordTags
            .Where(kt => kt.KeywordId == keywordId)
            .Include(k => k.Tag)
            .Select(k => new TagReqDto()
            {
                TagName = k.Tag.TagName,
                Id = k.Tag.Id,
            }).ToListAsync();


        return new SingleResDto<List<TagReqDto>>()
        {
            Data = tags
        };
    }

    public async Task RemoveKeywordTag(int keywordId, int tagId)
    {
        var keywordTag = await _context.KeywordTags
            .Where(k => k.KeywordId == keywordId && k.TagId == tagId)
            .FirstOrDefaultAsync();

        _context.Remove(keywordTag);

        await _context.SaveChangesAsync();
    }

    public async Task AddKeywordUrl(string link, int keywordId, int crawlDateId)
    {
        var keywordUrl = await _context.KeywordUrls
            .Where(u => u.Url == link && u.KeywordId == keywordId)
            .FirstOrDefaultAsync();

        if (keywordUrl == null)
        {
            var url = new KeywordUrl()
            {
                CrawlDateId = crawlDateId,
                KeywordId = keywordId,
                Url = link,
                CreationDateTime = DateTime.Now,
                ModificationDateTime = DateTime.Now
            };

            await _context.KeywordUrls.AddAsync(url);

            await _context.SaveChangesAsync();

            _logger.LogWarning("URL save successfully");
        }
    }

    public async Task GetKeywordsSearchVolume()
    {
        var keywordCount = _context.Keywords.Count();

        for (int i = 0; i <= keywordCount; i = i + 1000)
        {
            var keywords = await _context.Keywords
                .Select(k => k.Query)
                .Skip(i)
                .Take(1000)
                .ToListAsync();


            List<string> kwlist = new List<string>();

            List<string> filteredKeywords = keywords
                .Where(k => k.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length <= 10).ToList();


            var data = new
            {
                apikey = "214ff20c19d98d12d63280eb9867d002bf3985b8",
                keyword = filteredKeywords,
                metrics_location = new int[] { 2840 },
                metrics_language = new string[] { "en" },
                metrics_network = "googlesearchnetwork",
                metrics_currency = "USD",
                output = "json"
            };


            var jsonData = JsonConvert.SerializeObject(data);

            var options = new RestClientOptions("https://api.keywordtool.io/v2/search/volume/google");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddJsonBody(jsonData, false);
            var response = await client.PostAsync(request);

            var results = JsonConvert.DeserializeObject<KeywordToolDto>(response.Content);


            await AddKeywordsSerachVolume(results);
        }
    }

    public async Task AddKeywordsSerachVolume(KeywordToolDto data)
    {
        List<KeywordSearchVolume> svList = new List<KeywordSearchVolume>();

        foreach (var item in data.Results)
        {
            var keyword = await _context.Keywords.Where(k => k.Query == item.Key).FirstOrDefaultAsync();

            if (keyword != null && !_context.KeywordSearchVolumes.Any(x => x.KeywordId == keyword.Id))
            {
                if (!svList.Any(x => x.KeywordId == keyword.Id))
                {
                    svList.Add(new KeywordSearchVolume()
                    {
                        KeywordId = keyword.Id,
                        SearchVolume = item.Value.Volume.ToString() ?? "0"
                    });
                }
            }
        }

        await _context.KeywordSearchVolumes.AddRangeAsync(svList);
        await _context.SaveChangesAsync();
    }

    public async Task<string> ExportKeywordsToExcel(RankDto rank, int catId)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Keywords");

            // Define headers
            worksheet.Cell(1, 1).Value = "Keyword ID";
            worksheet.Cell(1, 2).Value = "Query";
            worksheet.Cell(1, 3).Value = "Current Position";
            worksheet.Cell(1, 4).Value = "Avg Position";
            worksheet.Cell(1, 5).Value = "Search Volume";
            int i = 6;
            foreach (var site in rank.Sites)
            {
                worksheet.Cell(1, i).Value = site.SiteName;

                i++;
            }


            // worksheet.Cell(1, i).Value = "URL";
            // Add more headers as necessary...

            int currentRow = 2;
            foreach (var query in rank.Queries)
            {
                // worksheet.Cell(currentRow, 1).Value = query.KeywordId;
                // worksheet.Cell(currentRow, 2).Value = query.Query;
                // worksheet.Cell(currentRow, 3).Value =
                //     query.CurrentPosition == null ? "-" : query.CurrentPosition.ToString();
                // worksheet.Cell(currentRow, 4).Value = query.AvgPosition.ToString();
                // worksheet.Cell(currentRow, 5).Value =  string.IsNullOrEmpty(query.SearchVolume) ? "0" : query.SearchVolume.ToString();

                int f = 6;


                // foreach (var site in rank.Sites)
                // {
                //         worksheet.Cell(currentRow, f).Value = query.Ranks.Where(x => x.SiteId == site.Id).Last().Position == null ? "-" : 
                //             query.Ranks.Where(x => x.SiteId == site.Id).Last().Position.ToString();
                //         f++;
                // }
                

                // var keywordUrl = query.KeywordUrls.FirstOrDefault(x => x.Url.Contains(_configuration.GetSection("DomainName").ToString()));
                // string url = keywordUrl?.Url ?? "";
                // // Add more cells as necessary...
                // worksheet.Cell(currentRow, f).Value = string.IsNullOrEmpty(url) ? "-" : url;
                currentRow++;
            }

            // Adjust columns to fit
            worksheet.Columns().AdjustToContents();

            // Save the workbook to a file

            var fileName =
                $"{_context.KeywordGroups.Where(x => x.Id == catId).FirstOrDefault().GroupTitle}-" +
                $"{DateTime.Now.ToShortDateString().Replace(" AP","").Replace("/","-")}-" +
                $"{Guid.NewGuid().ToString()}";
            
            workbook.SaveAs($"wwwroot/{fileName}.xlsx");

            await _context.ExcelExports.AddAsync(new ExcelExport()
            {
                KeywordGroupId = catId,
                FilePath = fileName
            });

            await _context.SaveChangesAsync();
            return fileName;
        }
        
        
        
    }

    public async Task DownloadHtml(int catId)
    {
        // var keywordsWithUrls = await _context.Keywords
        //     .Where(k => k.KeywordGroupId == catId)
        //     .Include(k => k.KeywordUrls)
        //     .ToListAsync();
        //
        // _logger.LogWarning("Downalod Html -> Keyword Service -> total count {@totalCounr}",keywordsWithUrls.Count);
        //
        // var keywords = keywordsWithUrls.Select(k => 
        //     new Keyword 
        //     {
        //         Id = k.Id,
        //         KeywordUrls = k.KeywordUrls.Distinct().ToList()
        //     }).ToList();
        //
        //
        // foreach (var keyword in keywords)
        // {
        //     foreach (var url in keyword.KeywordUrls)
        //     {
        //         var baseDomain = URLProcessor.GetMainDomain(url.Url);
        //        var res = await _htmlService.DownloadHtmlAsync(new Uri(url.Url), baseDomain);
        //
        //        await SaveDownloadedHtml(new KeywordHtml()
        //        {
        //            CrawlDateId = url.CrawlDateId,
        //            KeywordId = url.KeywordId,
        //            SiteId = _context.Sites.Where(x => x.SiteUrl.Contains(baseDomain) && x.KeywordGroupId == catId)
        //                .FirstOrDefault().Id,
        //            HtmlUrl = res
        //        });
        //     }
        // }

        var keywords = await _context.Keywords.Where(x => x.KeywordGroupId == catId)
            .Include(x => x.KeywordUrls.Where(x => x.Url.Contains(_configuration.GetSection("DomainName").ToString())))
            .ToListAsync();

        _logger.LogWarning("Download Html -> Keyword Service -> total count {@totalCount}", keywords.Count);

        var maxConcurrentTasks = 20; // Adjust based on your requirements
        var semaphore = new SemaphoreSlim(maxConcurrentTasks);
        int processedCount = 0; // Counter for processed elements

        var tasks = keywords.SelectMany(keyword => keyword.KeywordUrls.Select(async item =>
        {
            await semaphore.WaitAsync();

            try
            {
                var response = await _htmlService.DownloadHtmlAsync(new Uri(item.Url), "google");

                if (response.Contains("گوشی") || response.Contains("موبایل"))
                {
                    keyword.KeywordGroupId = 7;

                    lock (_context)
                    {
                        _context.Keywords.Update(keyword);
                        _context.SaveChanges();
                    }
                }

                // Increment the counter in a thread-safe manner
                Interlocked.Increment(ref processedCount);
                _logger.LogWarning("Item processed: {@processedCount}", processedCount);
            }
            finally
            {
                semaphore.Release();
            }
        })).ToList();

        await Task.WhenAll(tasks);

        // Save changes to the database after all tasks are complete


        _logger.LogWarning("Total items processed: {@processedCount}", processedCount);
    }

    public async Task SaveDownloadedHtml(KeywordHtml html)
    {
        await _context.KeywordHtmls.AddAsync(html);
        await _context.SaveChangesAsync();
    }
}