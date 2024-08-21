using System.Data;
using System.Globalization;
using System.Net;
using BusinessLayer.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Google.Apis.Auth.OAuth2;
using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Polly;
using Polly.Contrib.WaitAndRetry;
using UtilityLayer.Convertors;
using Exception = System.Exception;

namespace BusinessLayer.Services;

public class GoogleSearchConsoleService : IGoogleSearchConsoleService
{
    private readonly SeoToolDbContext _context;
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly ILogger<GoogleSearchConsoleService> _logger;
    private readonly IElasticClient _elasticClient;
    private readonly IConfiguration _configuration; 

    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
        Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(x => x.StatusCode is HttpStatusCode.InternalServerError or HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromMinutes(1), 10));

    private string CLIENT_SECRETS_PATH = "client_secret_755488886296.json";

    public GoogleSearchConsoleService(SeoToolDbContext context, IBackgroundJobClient backgroundJobs,
        ILogger<GoogleSearchConsoleService> logger, IElasticClient elasticClient, IConfiguration configuration)
    {
        _context = context;
        _backgroundJobs = backgroundJobs;
        _logger = logger;
        _elasticClient = elasticClient;
        _configuration = configuration;
    }


    public SearchConsoleService AuthorizeGoogleCreds()
    {
        var credential = GoogleCredential.FromFile(CLIENT_SECRETS_PATH)
            .CreateScoped("https://www.googleapis.com/auth/webmasters.readonly");

        var service = new SearchConsoleService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Rank Tracker"
        });

        return service;
    }

    public SearchAnalyticsQueryResponse ExecuteRequest(SearchConsoleService service, string propertyUri,
        SearchAnalyticsQueryRequest request)
    {
        SearchanalyticsResource.QueryRequest queryRequest = service.Searchanalytics.Query(request, propertyUri);
        return queryRequest.Execute();
    }


    public async Task<string> ExtractData(string site, bool unlimited, string startDateString, string endDateString,
        string output)
    {
        try
        {
            if (unlimited)
            {
                var now = DateTime.Now.AddDays(-3);
                var startDateTime = new DateTime(2022, 10, 13);
                _logger.LogWarning("Last Day of crawling is: {@date}",
                    now.ToString("yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture));
            
                // Generate list of dates
                List<string> dates = GenerateDateList(startDateTime, now);
            
                foreach (var date in dates)
                {
                    _logger.LogWarning($"Start date at beginning: {date}");
            
                    SearchConsoleService webmastersService = AuthorizeGoogleCreds();
                    await ProcessSearchConsoleData(webmastersService, site, date);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Error at processing search console data: {@e}", e.Message);
            throw;
        }

        return $"Search console finish";
    }


    public string GetDomainName(string startUrl)
    {
        Uri uri = new Uri(startUrl);
        string domainName = uri.Host;
        domainName = domainName.Replace('.', '_');
        return domainName;
    }

    public async Task AddSearchConsoleData(List<SearchConsoleData> data)
    {
        try
        {
            _logger.LogWarning("Start Adding Keywords to Database");

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            var performances = new List<SearchPerformance>();

            foreach (var item in data)
            {
                int keywordId = await EnsureKeywordExistsAndGetId(item.Query);
        
                performances.Add(new SearchPerformance
                {
                    SearchKeywordId = keywordId,
                    Date = Convert.ToDateTime(item.Date),
                    Clicks = Convert.ToInt32(item.Clicks),
                    Impressions = Convert.ToInt32(item.Impressions),
                    CTR = float.Parse(item.Ctr),
                    Position = float.Parse(item.Position),
                    Page = item.Page
                });
            }

            await _context.SearchPerformances.AddRangeAsync(performances);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<int> EnsureKeywordExistsAndGetId(string query)
    {
        return await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable))
            {
                var existingKeyword = await _context.SearchKeywords
                    .SingleOrDefaultAsync(sk => sk.Query == query);

                if (existingKeyword != null)
                {
                    await transaction.CommitAsync();
                    return existingKeyword.Id;
                }

                try
                {
                    var keywordEntry = (await _context.SearchKeywords
                        .AddAsync(new SearchKeyword { Query = query })).Entity;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return keywordEntry.Id;
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();

                    // Log the exception or handle it as needed
                    // Assuming DbUpdateException is due to a unique constraint violation,
                    // you might want to handle it specifically here.
                
                    // Re-fetching the keyword in case the insertion was attempted by
                    // another concurrent call which succeeded before this transaction could commit.
                    existingKeyword = await _context.SearchKeywords
                        .SingleOrDefaultAsync(sk => sk.Query == query);
                    if (existingKeyword != null)
                    {
                        return existingKeyword.Id;
                    }
                
                    // If after the exception, the keyword is still not found, rethrow or handle as needed.
                    throw;
                }
            }
        });
    }

        public async Task<(List<SearchConsoleKeywordDto>, List<SearchConsoleKeywordRankDto>)> ProcessSearchConsoleData(
        SearchConsoleData keyword)
    {
        var searchConsoleKeywords = new List<SearchConsoleKeywordDto>();
        var searchConsoleKeywordRank = new List<SearchConsoleKeywordRankDto>();

        var existingKeyword = await _elasticClient.SearchAsync<SearchConsoleKeywordDto>(s => s
            .Index("searchconsolekeywords")
            .Query(q => q
                .MatchPhrase(m => m
                    .Field(f => f.Query)
                    .Query(keyword.Query))));

        if (existingKeyword.Documents.Count == 0)
        {
            var searchConsoleKeyword = new SearchConsoleKeywordDto
                { Id = Guid.NewGuid().ToString(), Query = keyword.Query };

            searchConsoleKeywords.Add(searchConsoleKeyword);

            var rank = new SearchConsoleKeywordRankDto
            {
                QueryId = searchConsoleKeyword.Id,
                Date = keyword.Date,
                Clicks = keyword.Clicks,
                Ctr = keyword.Ctr,
                Impressions = keyword.Impressions,
                Position = keyword.Position,
                Page = keyword.Page
            };

            searchConsoleKeywordRank.Add(rank);
        }
        else
        {
            var rank = new SearchConsoleKeywordRankDto
            {
                QueryId = existingKeyword.Documents.First().Id,
                Date = keyword.Date,
                Clicks = keyword.Clicks,
                Ctr = keyword.Ctr,
                Impressions = keyword.Impressions,
                Position = keyword.Position,
                Page = keyword.Page
            };
            searchConsoleKeywordRank.Add(rank);
        }

        return (searchConsoleKeywords, searchConsoleKeywordRank);
    }

    public void CreateReadDataFromSearchConsoleRequest(string startDate, string endDate)
    {
        string site = _configuration.GetSection("DomainName").ToString();
        string creds = _configuration.GetSection("Creds").ToString();
        string output = "gsc_data.csv";


        _backgroundJobs.Schedule(() => ExtractData(site, true, startDate, endDate, output),
            DateTimeOffset.Now.AddMinutes(1));
    }

    public List<string> GenerateDateList(DateTime startDate, DateTime endDate)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        List<string> dates = new List<string>();

        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            dates.Add(date.ToString("yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture));
        }

        return dates;
    }

    public async Task ProcessSearchConsoleData(SearchConsoleService webmastersService, string site, string date)
    {
        var endDate = date;
        var startDate = date;
        Dictionary<string, List<string>> scDict = InitializeSearchConsoleDictionary();

        int maxRows = 25000;
        int numRows = 0;
        string status = "";

        while (status != "Finished")
        {
            List<SearchConsoleData> searchConsoleData = new List<SearchConsoleData>();

            var request = new SearchAnalyticsQueryRequest
            {
                StartDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd"),
                EndDate = Convert.ToDateTime(endDate).ToString("yyyy-MM-dd"),
                Dimensions = new List<string> { "date", "page", "query" },
                RowLimit = maxRows,
                StartRow = numRows
            };

            var response = ExecuteRequest(webmastersService, site, request);

            if (response.Rows != null)
            {
                if (response.Rows.Count > 0)
                {
                    foreach (var row in response.Rows)
                    {
                        if (!searchConsoleData.Any(s => s.Query == row.Keys[2]))
                        {
                            searchConsoleData.Add(new SearchConsoleData()
                            {
                                Date = row.Keys[0],
                                Page = row.Keys[1],
                                Query = row.Keys[2],
                                Clicks = row.Clicks.ToString(),
                                Ctr = row.Ctr.ToString(),
                                Impressions = row.Impressions.ToString(),
                                Position = row.Position.ToString()
                            });
                        }
                    }
                    numRows += response.Rows.Count;
                }
            }
            else
            {
                status = "Finished";
            }

            _logger.LogError("Successful at {@rows}: ", numRows);
            


            _logger.LogWarning($"{searchConsoleData.Count} keyword founds in second filter");
            await AddSearchConsoleData(searchConsoleData);
        }
    }

    public async Task ReadGoogleSearchConsoleDataFromElastic()
    {
        var searchResponse = await _elasticClient.SearchAsync<SearchConsoleData>(s => s
                .Index("keywords")
                .Size(10000) // Number of documents per batch
                .Scroll("10m") // Scroll timeout
        );

        int page = 1;

        while (searchResponse.IsValid && searchResponse.Documents.Any())
        {
            _logger.LogWarning("page is: {page} & total count is: {totalcount}", page, page * 10000);

            await AddGoogleSearchConsoleDataToDb(searchResponse.Documents.ToList());

            var scrollResponse = await _elasticClient.ScrollAsync<SearchConsoleData>("5m", searchResponse.ScrollId);

            if (!scrollResponse.IsValid)
            {
                // Handle error if needed
                throw new InvalidOperationException(
                    $"Error scrolling. Status: {scrollResponse.ApiCall.HttpStatusCode}");
            }

            searchResponse = scrollResponse;
            page += 1;
        }


        Console.WriteLine("test");
    }

    public async Task AddGoogleSearchConsoleDataToDb(List<SearchConsoleData> data)
    {
//         #region Add TO Database
//
//         _context.Database.SetCommandTimeout(800);
//
//         var existingKeywords = await _context.SearchConsoleKeywords
//             .Select(k => k.Query)
//             .ToListAsync();
//
//         _logger.LogWarning("existing keywords count: {count}", existingKeywords.Count);
//
//         var existingKeywordsHashSet = existingKeywords
//             .ToHashSet(StringComparer.OrdinalIgnoreCase);
//
//         _logger.LogWarning("existing keywords count: {count}", existingKeywords.Count);
//
// // Filter out duplicates before adding
//         var keywordsToAdd = data
//             .Select(item => item.Query)
//             .Where(keyword => !existingKeywordsHashSet.Contains(keyword, StringComparer.OrdinalIgnoreCase))
//             .Distinct(StringComparer.OrdinalIgnoreCase)
//             .ToList();
//
//         _logger.LogWarning("keywords to add count: {count}", keywordsToAdd.Count);
//
//         if (keywordsToAdd.Count > 0)
//         {
//             // Only add new keywords to the database
//             _context.SearchConsoleKeywords.AddRange(
//                 keywordsToAdd.Select(keyword => new SearchConsoleKeyword { Query = keyword })
//             );
//
//             await _context.SaveChangesAsync();
//         }
//
//
//         var keywordDictionary = await _context.SearchConsoleKeywords
//             .Select(k => new { k.Query, k.Id })
//             .ToListAsync();
//
//
//         var dateDictionary = _context.SearchConsoleDates.ToDictionary(d => d.Date, d => d.Id);
//
//         var keywordRanks = data.Select(item =>
//             {
//                 var keyword = keywordDictionary.Where(kd => kd.Query == item.Query).FirstOrDefault();
//
//
//                 if (keyword != null &&
//                     dateDictionary.TryGetValue(item.Date + " 00:00:00", out var dateId))
//                 {
//                     return new SearchConsoleKeywordRank
//                     {
//                         SearchConsoleKeywordId = keyword.Id,
//                         SearchConsoleDateId = dateId,
//                         Clicks = item.Clicks,
//                         Ctr = item.Ctr,
//                         Impressions = item.Impressions,
//                         Position = item.Position
//                     };
//                 }
//                 else
//                 {
//                     _logger.LogWarning("Keyword or Date not found for Query: {query}, Date: {date}", item.Query,
//                         item.Date);
//                     return null;
//                 }
//             })
//             .Where(rank => rank != null)
//             .ToList();
//
//         _logger.LogWarning("keywords rank count: {count}", keywordRanks.Count);
//
//         var urls = data.Select(item =>
//             {
//                 var keyword = keywordDictionary.Where(kd => kd.Query == item.Query).FirstOrDefault();
//
//                 if (keyword != null)
//                 {
//                     return new SearchConsoleUrl
//                     {
//                         SearchConsoleKeywordId = keyword.Id,
//                         Page = item.Page
//                     };
//                 }
//                 else
//                 {
//                     _logger.LogWarning("Keyword not found for Query: {query}", item.Query);
//                     return null;
//                 }
//             })
//             .Where(url => url != null)
//             .ToList();
//
//         _logger.LogWarning("urls to add count: {count}", urls.Count);
//
//         _context.SearchConsoleKeywordRanks.AddRange(keywordRanks);
//         _context.SearchConsoleUrls.AddRange(urls);
//
//         await _context.SaveChangesAsync();
//
//         #endregion
    }

    public async Task GetFullReportOfSearchConsole(string period)
    {

        var reports = await GetDailyData(period);
        
        await AddSearchConsoleReports(reports);

    }

    public async Task<List<SearchConsoleReport>> GetDailyData(string period)
    {
        var rangDate = DateConvertor.GetDateRange(period);
        
        var request = new SearchAnalyticsQueryRequest
        {
            StartDate = rangDate.startDate,
            EndDate = rangDate.endDate,
            Dimensions = new List<string> { "date" },
        };

        SearchConsoleService webmastersService = AuthorizeGoogleCreds();

        var response = ExecuteRequest(webmastersService, _configuration.GetSection("DomainName").ToString(), request);

        List<SearchConsoleReport> reports = new List<SearchConsoleReport>();
        foreach (var item in response.Rows)
        {
            reports.Add(new SearchConsoleReport(){Clicks = Convert.ToDouble(item.Clicks),
                Impressions = Convert.ToDouble(item.Impressions),
                Ctr = Convert.ToDouble(item.Ctr),
                AvgPosition = Convert.ToDouble(item.Position),
                Date = item.Keys[0]
            });
        }

        return reports;
    }

    public async Task<SearchConsoleReportDto> GetSearchConsoleReport(string period)
    {
        var rangDate = DateConvertor.GetDateRange(period);
        var dates = DateConvertor.GetAllDatesInRange(rangDate.startDate, rangDate.endDate);

        var reports = await _context.SearchConsoleReports
            .Where(x => dates.Any(d => x.Date == d))
            .ToListAsync();


        var result = new SearchConsoleReportDto()
        {
            AllClick = reports.Sum(r => r.Clicks),
            AllImpression = reports.Sum(r => r.Impressions),
            AvgPosition = reports.Average(r => r.AvgPosition),
            AvgCtr = reports.Average(r => r.Ctr) * 100,
            Lables = dates,
            SearchConsoleReports = reports
        };


        return result;
    }

    public async Task AddSearchConsoleReports(List<SearchConsoleReport> reports)
    {
        foreach (var report in reports)
        {
            if (!_context.SearchConsoleReports.Any(x => x.Date == report.Date && x.Clicks == report.Clicks && x.Impressions == report.Impressions))
            {
               await _context.AddAsync(report);
               await _context.SaveChangesAsync();
            }
        }
    }

    public async Task<PaginitionResDto<List<SearchConsoleResponse>>>  GetSearchConsoleKeywords(int pageSize, int pageIndex)
    {

        List<SearchConsoleResponse> response = new List<SearchConsoleResponse>();
        
        var searchResponse = await _elasticClient.SearchAsync<SearchConsoleKeywordDto>(s => s
                .Index("searchconsolekeywords")
                .Skip((pageIndex - 1) * pageSize)
                .Size(pageSize) // Number of documents per batch
                .Scroll("10m") // Scroll timeout
        );
        
        foreach (var item in searchResponse.Documents)      
        {
            var rank = _elasticClient.Search<SearchConsoleKeywordRankDto>(s => s
                .Index("searchconsole")
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.QueryId.Suffix("keyword")) // Use the .Suffix("keyword") to target the keyword sub-field
                        .Value(item.Id)
                    )
                )
                .Sort(so => so
                    .Descending(f => f.Date)
                )
                .Size(1)
            );

            if (rank.IsValid && rank.Documents.Any())
            {
                var lastItem = rank.Documents.First();
                response.Add(new SearchConsoleResponse(){Keyword = item,Report = lastItem});
                Console.WriteLine($"Date: {lastItem.Date}, Clicks: {lastItem.Clicks}, Position: {lastItem.Position}");
            }
            else
            {
                Console.WriteLine(rank.DebugInformation);
                Console.WriteLine("No results found or query failed.");
            }
        }
        
        var countResponse = _elasticClient.Count<SearchConsoleKeywordDto>(c => c
            .Index("searchconsolekeywords")
            .Query(q => q
                .MatchAll()
            )
        );
        return new PaginitionResDto<List<SearchConsoleResponse>>()
        {
            Data = response,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = Convert.ToInt32(countResponse.Count),

        };
    }

    public async Task<SearchConsoleTypeReportDto> GetFullTypeReport(string period)
    {
        var rangDate = DateConvertor.GetDateRange(period);
        
        string[] searchTypes = {"WEB", "IMAGE", "VIDEO", "NEWS"};

        List<SearchConsoleType> searchConsoleTypeResults = new List<SearchConsoleType>();

        foreach (var searchType in searchTypes)
        {
            var request = new SearchAnalyticsQueryRequest
            {
                StartDate = rangDate.startDate,
                EndDate = rangDate.endDate,
                SearchType = searchType
            };
            
            SearchConsoleService webmastersService = AuthorizeGoogleCreds();

            var response = ExecuteRequest(webmastersService, _configuration.GetSection("DomainName").ToString(), request);

            List<SearchConsoleReport> reports = new List<SearchConsoleReport>();
        
            foreach (var item in response.Rows)
            {
                reports.Add(new SearchConsoleReport(){Clicks = Convert.ToDouble(item.Clicks),
                    Impressions = Convert.ToDouble(item.Impressions),
                    Ctr = Convert.ToDouble(item.Ctr),
                    AvgPosition = Convert.ToDouble(item.Position),
                });
            }

            SearchConsoleReport report = new SearchConsoleReport()
            {
                Clicks = reports.Sum(x => x.Clicks),
                Impressions = reports.Sum(x => x.Impressions),
                Ctr = reports.Average(x => x.Ctr),
                AvgPosition = reports.Average(x => x.AvgPosition)
            };
            
            searchConsoleTypeResults.Add(new SearchConsoleType() {TypeName = searchType,Type = report});
        }


        return new SearchConsoleTypeReportDto()
        {
            StartDate = rangDate.startDate,
            EndDate = rangDate.endDate,
            report = searchConsoleTypeResults
        };


    }

    string NormalizeKeyword(string keyword)
    {
        // Example: Replace half space (non-breaking space) with regular space
        return keyword.Replace('\u200C', ' ').Trim();
    }

    // Helper method to initialize the search console dictionary
    Dictionary<string, List<string>> InitializeSearchConsoleDictionary()
    {
        return new Dictionary<string, List<string>>
        {
            { "date", new List<string>() },
            { "page", new List<string>() },
            { "query", new List<string>() },
            { "clicks", new List<string>() },
            { "ctr", new List<string>() },
            { "impressions", new List<string>() },
            { "position", new List<string>() }
        };
    }
}