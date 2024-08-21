using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using BusinessLayer.Repositories;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nest;

namespace BusinessLayer.Services;

public class ReportService : IReportService
{

    private readonly SeoToolDbContext _context;
    private readonly IElasticService<ReportDto> _keywordElasticService;
    private readonly IMapper _mapper;
    private readonly IElasticClient _elasticClient;
    private readonly IGoogleSearchConsoleService _googleSearchConsoleService;
    private readonly IConfiguration _configuration; 


    public ReportService(SeoToolDbContext context, IElasticService<ReportDto> keywordElasticService, IMapper mapper, IElasticClient elasticClient, IGoogleSearchConsoleService googleSearchConsoleService, IConfiguration configuration)
    {
        _context = context;
        _keywordElasticService = keywordElasticService;
        _mapper = mapper;
        _elasticClient = elasticClient;
        _googleSearchConsoleService = googleSearchConsoleService;
        _configuration = configuration;
    }

    public async Task<ReportDto> CreateReport(string keywordType, int keywordGroupId, int siteId ,string rank, int? tagId, int? searchVolume, bool branded,CrawlDate date)
    {
        if (keywordType == "keyword")
        {


            var result = new ReportDto();
            var keywords = new List<Keyword>();
            
            var queryableKeywords = _context.Keywords.AsQueryable();
            // Query the keywords
            switch (rank)
            {
                
                case "Top3":
                    

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == keywordGroupId &&
                                    k.Ranks.Any(r => r.Position <= 3 && r.CrawlDateId == date.Id) &&
                                    k.KeywordGroup.Sites.Any(x => x.Id == siteId));

                    if (tagId.HasValue)
                    {
                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
                    }

                    keywords = await queryableKeywords
                        .Include(k => k.KeywordTags)
                        .ThenInclude(k => k.Tag)
                        .Include(k => k.KeywordSearchVolume)
                        .Include(k => k.Ranks.Where(r => r.CrawlDateId == date.Id))
                        .ThenInclude(s => s.Site)
                        .ToListAsync();
                    
                    break;
                
                case "Top10":
                    

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == keywordGroupId &&
                                    k.Ranks.Any(r => r.Position <= 3 && r.Position <= 10&& r.CrawlDateId == date.Id) &&
                                    k.KeywordGroup.Sites.Any(x => x.Id == siteId));

                    if (tagId.HasValue)
                    {
                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
                    }

                    keywords = await queryableKeywords
                        .Include(k => k.KeywordTags)
                        .ThenInclude(k => k.Tag)
                        .Include(k => k.KeywordSearchVolume)
                        .Include(k => k.Ranks.Where(r => r.CrawlDateId == date.Id))
                        .ThenInclude(s => s.Site)
                        .ToListAsync();
                    
                    break;
                
                case "All":

                    queryableKeywords = queryableKeywords
                        .Where(k => k.KeywordGroupId == keywordGroupId &&
                                    k.Ranks.Any(r => r.CrawlDateId == date.Id) &&
                                    k.KeywordGroup.Sites.Any(x => x.Id == siteId));

                    if (tagId.HasValue)
                    {
                        queryableKeywords = queryableKeywords
                            .Where(k => k.KeywordTags.Any(t => t.TagId == tagId.Value));
                    }

                    keywords = await queryableKeywords
                        .Include(k => k.KeywordTags)
                        .ThenInclude(k => k.Tag)
                        .Include(k => k.KeywordSearchVolume)
                        .Include(k => k.Ranks.Where(r => r.CrawlDateId == date.Id && r.SiteId == siteId))
                        .ThenInclude(s => s.Site)
                        .ToListAsync();
                    
                    break;
            }

            var query = await GenerateReportQueryList(keywords, keywordGroupId, siteId, tagId);
            var identifier = CreateUniqueIdentifier(keywordType, keywordGroupId, siteId, rank, tagId,
                searchVolume, branded,date.Id);
            result =  await GenerateKeywordGroupHistory(query, date.CrawlDateTime, keywordGroupId,identifier);




            return result;
        }

        // Handle other keyword types if needed

        return null; // Return null or a default ReportDto if the keyword type does not match
    }

    public async Task<ReportDto> GenerateKeywordGroupHistory(List<ReportQueryDto> query, DateTime crawlDate, int groupId,string identifier)
    {

            var keywordGroupHistory = new ReportDto()
            {

                Id = identifier,
                TotalCount = query.Count,
                Avg = Math.Round(query
                    .Sum(q => Convert.ToDouble(q.CurrentPosition)) / query.Count, 2),
                CrawlDate = crawlDate
            };

            await _keywordElasticService.CreateDocumentAsync(keywordGroupHistory,"reports");
            
            return keywordGroupHistory;
        
        
    }

    public async Task<List<ReportQueryDto>> GenerateReportQueryList(List<Keyword> keywords,int catId,int siteId,int? tagId)
    {
        return keywords.Select(keyword => new ReportQueryDto
        {
            KeywordId = keyword.Id,
            Query = keyword.Query,
            AvgPosition = keyword.Ranks
                .Where(r => r.SiteId == siteId)
                .Select(r => r.Position)
                .DefaultIfEmpty() // Ensures that an empty sequence results in a default value (0.0 for double)
                .Average()
                .ToString("0.00"),
            CurrentPosition = keyword.Ranks
                .OrderByDescending(r => r.CreationDateTime)
                .FirstOrDefault()?.Position.ToString(),
            LastTrackTime = keyword.Ranks
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
    }

    public string CreateUniqueIdentifier(string keywordType, int keywordGroupId, int siteId, string rank, int? tageId,
        int? searchVolume, bool branded,int crawlId)
    {
        string input = $"{keywordType}-{keywordGroupId}-{siteId}--{rank}--{tageId}--{searchVolume}--{branded}--{crawlId}";
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }

    public async Task<ReportDto> GetReport(string keywordType, int keywordGroupId, int siteId, string rank, int? tagId, int? searchVolume,
        bool branded)
    {
        
        
        var crawlDates = await _context.CrawlDates.OrderByDescending(c => c.Id).ToListAsync();

            

        var date = new CrawlDate();

        foreach (var cralwDate in crawlDates)
        {
            var check = await _context.Ranks.AnyAsync(r =>
                r.CrawlDateId == cralwDate.Id && r.Keyword.KeywordGroupId == keywordGroupId);

            if (check)
            {
                date = cralwDate;
                break;
            }
        }
        
        var report = new ReportDto();

        var identifire = CreateUniqueIdentifier(keywordType, keywordGroupId, siteId, rank, tagId,
            searchVolume, branded,date.Id);

        var response = await _elasticClient.GetAsync<ReportDto>(identifire,i => i.Index("reports"));

        if (response.Source == null)
        {
            return await CreateReport(keywordType, keywordGroupId, siteId, rank, tagId,
                searchVolume, branded,date);
        }
        
        return response.Source;
    }

    public async Task<DashboardDto> GenerateDashboardReport()
    {

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        var brandedPattern = @"dijikala|دیحی کالا|digi kala|ديجيكالا|digi|djkala|دبجی کالا|دیجیکالا|دیجی کالا|دی جی کالا|دی جی|دیجی|digikala|ديجي كالا|دي جي كالا|دجی کالا|dgkala|dj kala|ديجى كالا";
        
        Regex brandedRegex = new Regex(brandedPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        var dailyReport = await _googleSearchConsoleService.GetDailyData("last28days");
        var lastMonthReport = await _googleSearchConsoleService.GetDailyData("lastmonth");
        
        var report = new DashboardDto();

        var gsc = _context.SearchKeywords.AsQueryable();

        var keywords = _context.Keywords.AsQueryable();

        var performance = _context.SearchPerformances.AsQueryable();
        
        report.GscKeywordCount = await gsc.CountAsync();
        
        report.RtKeywordCount = await keywords.CountAsync();
        
        var allSearchKeywords = await gsc.ToListAsync();
        
        report.BrandedGscKeywordCount = allSearchKeywords
            .Where(g => brandedRegex.IsMatch(g.Query)) // Replace "%pattern%" with your actual pattern
            .Count();
        
        report.NoneBrandedGscKeywordCount = allSearchKeywords.Where(g => !brandedRegex
                .IsMatch(g.Query))
            .Count();

        var bestDayThisMonth = dailyReport.OrderByDescending(r => r.Clicks).FirstOrDefault();
        var bestDayLastMonth = lastMonthReport.OrderByDescending(r => r.Clicks).FirstOrDefault();
        
        report.BestDayThisMonth = Convert.ToDateTime(bestDayThisMonth.Date);
        
        report.BestDayLastMonth = Convert.ToDateTime(bestDayLastMonth.Date);

        report.BestClikThisMonth = bestDayThisMonth.Clicks;

        report.BestImpThisMonth = bestDayThisMonth.Impressions;
        
        report.BestClickLastMonth = bestDayLastMonth.Clicks;

        report.BestImpLastMonth = bestDayLastMonth.Impressions;

        var allKeywords = await keywords.ToListAsync();
        
        report.BrandedRtKeywordCount = allKeywords.Where(g => brandedRegex.IsMatch(g.Query))
            .Count();
        
        report.NoneBrandedRtKeywordCount = allKeywords.Where(g => !brandedRegex.IsMatch(g.Query))
            .Count();

        var allPerformance = await performance.Where(x =>
            x.Date == Convert.ToDateTime("2022-10-18")).ToListAsync();
        
        report.BrandedDailyClick = allPerformance.Where(x =>
               brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(s => s.Clicks);
        
        report.NoneBrandedDailyClick = allPerformance.Where(x =>
                !brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(s => s.Clicks);
        
        report.BrandedDailyImpression = allPerformance.Where(x =>
                brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(s => s.Impressions);
        
        report.NoneBrandedDailyImpression = allPerformance.Where(x =>
                !brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(s => s.Impressions);

        var startDate = Convert.ToDateTime("2022-10-18");
        var endDate = Convert.ToDateTime("2022-11-16");

        var monthPerformance = await performance
            .Where(c => c.Date >= startDate && c.Date <= endDate)
            .ToListAsync();
        
        report.BrandedMonthlyClick = monthPerformance
            .Where(x => brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(x => x.Clicks);
        
        report.NoneBrandedMonthlyClick = monthPerformance
            .Where(x => !brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(x => x.Clicks);
        
        report.BrandedMonthlyImpression = monthPerformance
            .Where(x => brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(x => x.Impressions);
        
        report.NoneBrandedMonthlyImpression = monthPerformance
            .Where(x => !brandedRegex.IsMatch(x.SearchKeyword.Query))
            .Sum(x => x.Impressions);
        
        return report;
    }

    
    
    
    public async Task<SearchConsoleDefalutDto> GetDailyOverview(string period)
    {
        var dailyReport = await _googleSearchConsoleService.GetDailyData(period);

        return new SearchConsoleDefalutDto()
        {
            Clicks = dailyReport.Sum(x => x.Clicks),
            Impression = dailyReport.Sum(x => x.Impressions),
            Ctr = dailyReport.Average(x => x.Ctr),
            AvgPosition = dailyReport.Average(x => x.AvgPosition)
        };
    }

    public async Task<List<KeywordCompareDto>> CompareKeywordRank()
    {
        // Project keywords and their last two ranks into a temporary object
        var keywordRankData = await _context.Keywords
            .Select(k => new
            {
                Keyword = k,
                LastTwoRanks = _context.Ranks
                    .Where(r => r.KeywordId == k.Id && r.Site.SiteUrl == _configuration.GetSection("DomainName").ToString())
                    .OrderByDescending(r => r.CrawlDateId)
                    .Take(2)
                    .ToList()
            })
            .ToListAsync();

        // Convert the temporary object into KeywordCompareDto, calculating the rank change
        var keywordRankChanges = keywordRankData
            .Where(x => x.LastTwoRanks.Count == 2) // Ensure there are exactly two ranks
            .Select(x =>
            {
                var ranks = x.LastTwoRanks.OrderBy(r => r.CrawlDateId).ToList(); // Ensure correct order
                var latestRank = ranks.Last();
                var previousRank = ranks.First(); // Since we only have two, this will be the previous

                return new KeywordCompareDto
                {
                    KeywordId = x.Keyword.Id,
                    Query = x.Keyword.Query,
                    RankChange = Math.Abs(latestRank.Position - previousRank.Position),
                    CurrentPosition = latestRank.Position,
                    PrevRank = previousRank.Position
                };
            })
            .OrderByDescending(dto => dto.RankChange)
            .TakeLast(10)
            .ToList();

        return keywordRankChanges;
        
    }
}