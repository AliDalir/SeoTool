using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using UtilityLayer.GoogleSheets;

namespace BusinessLayer.Repositories;

public interface IKeywordService
{
    public Task<ResponseDto> AddKeywordAsync(List<KeywordDto> keywords);

    public Task AddKeywordGroup(List<string> title);

    public Task AddSites(SiteReqDto site);

    public Task CreateTrackRequest(int? catId);
    
    public Task<ResponseDto> TrackKeywordsRankAsync(int? catId);

    public Task SaveSeperResponseToMongo(List<SerpApiDto> response, string id, int crawlDateId, int KeywordGroupId);

    public Task ProccessKeywordTracktion(int catId, int crawlDateId,List<Keyword> keywords,List<Site> sites);

    public Task ProcessSiteRankAsync(Site site, SerpApiDto res, Keyword keyword, int crawlDateId);

    public Task<List<SerpApiDto>> RequestKeywordRank(List<string> queries);


    public Task<List<NightWatchDto>> GetAllKeywordsFromNightWatch();

    public Task AddRank(BaseRankDto rank);

    // public Task<PaginitionResDto<RankDto>> GetAllRanks(int pageSize,int pageIndex);

    public Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsync(int catId, int pageSize, int pageIndex,string? rank, int? tagId);
    public Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsyncOptimized(int catId, int pageSize, int pageIndex,string? rank, int? tagId);
    public Task<string> GetRanksByCategoryForExcelExportAsync(int catId, int pageSize, int pageIndex,string? rank, int? tagId);
    public Task<PaginitionResDto<RankDto>> GenerateHistoricalDataByCatIdAsync(int catId, int pageSize, int pageIndex);

    public Task<List<QueryDto>> GenerateQueryDto(List<Keyword> keywords,int catId);
    public Task<List<QueryDto>> GenerateQueryDtoForExcelExport(List<Keyword> keywords,int catId);

    public Task<Tuple<List<Keyword>,int>> FindKeywordsForHistoricalData(int catId, int pageSize, int pageIndex,string? rank, int? tagId);
    public Task<Tuple<List<KeywordResponseDto>,int>> FindKeywordsForHistoricalDataOptimized(int catId, int pageSize, int pageIndex,string? rank, int? tagId);
    public Task<List<Site>> FindSitesForHistoricalData(int catId);

    public Task<PaginitionResDto<List<KeywordGroupDto>>> GetAllKeywordGroupsAsync();

    public Task RequestGenerateKeywordGroupHistory(int catId,int crawlDateId);
    
    public Task<KeywordGroupHistory> GenerateKeywordGroupHistory(List<QueryDto> query, int crawlDateId, int groupId);

    public Task<SingleResDto<List<SiteDto>>> GetKeywordSites(int keywordId);

    Task<SingleResDto<KeywordHistoryDto>> GetKeywordDetailAsync(int id);
    Task<SingleResDto<KeywordDto>> GetKeywordAsync(int id);
    Task<SingleResDto<List<KeywordDto>>> GetAllKeywordsAsync();

    public Task AddVerticalsWithCompetitors(List<VerticalWithCompetitorsDto> verticalWithCompetitors);

    public Task AddTag(List<TagDto> tags);

    public Task AddKeywordsWithTags(List<KeywordWithTagsDto> keywordWithTags);

    public Task AddTagToKeyword(KeywordTag keywordTag);

    public Task<SingleResDto<List<TagReqDto>>> GetAllTags();

    public Task<SingleResDto<List<TagReqDto>>> GetKeywordTagById(int keywordId);

    public Task RemoveKeywordTag(int keywordId, int tagId);

    public Task AddKeywordUrl(string link,int keywordId,int crawlDateId);

    public Task GetKeywordsSearchVolume();

    public Task AddKeywordsSerachVolume(KeywordToolDto data);

    public Task<string> ExportKeywordsToExcel(RankDto rank,int catId);

    public Task DownloadHtml(int catId);

    public Task SaveDownloadedHtml(KeywordHtml html);
    


}