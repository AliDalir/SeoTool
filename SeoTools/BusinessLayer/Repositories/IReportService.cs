using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;

namespace BusinessLayer.Repositories;

public interface IReportService
{
    public Task<ReportDto> CreateReport(string keywordType, int keywordGroupId, int siteId,string rank ,int? tageId, int? searchVolume,bool branded,CrawlDate date);
    
    public Task<ReportDto> GenerateKeywordGroupHistory(List<ReportQueryDto> query, DateTime crawlDate, int groupId,string identifier);

    public Task<List<ReportQueryDto>> GenerateReportQueryList(List<Keyword> keywords,int catId,int siteId,int? tagId);

    public string CreateUniqueIdentifier(string keywordType, int keywordGroupId, int siteId, string rank, int? tageId,
        int? searchVolume, bool branded,int crawlId);
    
    public Task<ReportDto> GetReport(string keywordType, int keywordGroupId, int siteId, string rank, int? tagId,
        int? searchVolume, bool branded);

    public Task<DashboardDto> GenerateDashboardReport();

    public Task<SearchConsoleDefalutDto> GetDailyOverview(string period);

    public Task<List<KeywordCompareDto>> CompareKeywordRank();
}