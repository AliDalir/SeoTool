using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Google.Apis.SearchConsole.v1;
using Google.Apis.SearchConsole.v1.Data;

namespace BusinessLayer.Repositories;

public interface IGoogleSearchConsoleService
{
    public SearchConsoleService AuthorizeGoogleCreds();

    public SearchAnalyticsQueryResponse ExecuteRequest(SearchConsoleService service, string propertyUri,
        SearchAnalyticsQueryRequest request);
    

    public Task<string> ExtractData(string site, bool unlimited, string startDateString, string endDateString , string output);

    public string GetDomainName(string startUrl);

    public Task AddSearchConsoleData(List<SearchConsoleData> data);

    public Task<int> EnsureKeywordExistsAndGetId(string query);

    public Task<(List<SearchConsoleKeywordDto>, List<SearchConsoleKeywordRankDto>)> ProcessSearchConsoleData(
        SearchConsoleData keyword);

    public void CreateReadDataFromSearchConsoleRequest(string startDate, string endDate);

    public List<string> GenerateDateList(DateTime startDate, DateTime endDate);

    public Task ProcessSearchConsoleData(SearchConsoleService webmastersService, string site, string date);

    public Task ReadGoogleSearchConsoleDataFromElastic();

    public Task AddGoogleSearchConsoleDataToDb(List<SearchConsoleData> data);


    public Task GetFullReportOfSearchConsole(string period);

    public Task<List<SearchConsoleReport>> GetDailyData(string period);
    public Task<SearchConsoleReportDto> GetSearchConsoleReport(string period);

    public Task AddSearchConsoleReports(List<SearchConsoleReport> reports);
    public Task<PaginitionResDto<List<SearchConsoleResponse>>> GetSearchConsoleKeywords(int pageSize,int pageIndex);

    public Task<SearchConsoleTypeReportDto> GetFullTypeReport(string period);

}