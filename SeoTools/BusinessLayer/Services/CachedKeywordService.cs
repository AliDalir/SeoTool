using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UtilityLayer.GoogleSheets;

namespace BusinessLayer.Services;

public class CachedKeywordService : IKeywordService
{

    private readonly KeywordService _decorated;

    private readonly IDistributedCache _distributedCache;

    public CachedKeywordService(KeywordService decorated, IDistributedCache distributedCache)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
    }

    public async Task<ResponseDto> AddKeywordAsync(List<KeywordDto> keywords)
    {
        return await _decorated.AddKeywordAsync(keywords);
    }

    public async Task AddKeywordGroup(List<string> title)
    {
        await _decorated.AddKeywordGroup(title);
    }

    public async Task AddSites(SiteReqDto site)
    {
        await _decorated.AddSites(site);
    }

    public async Task CreateTrackRequest(int? catId)
    {
        await _decorated.CreateTrackRequest(catId);
    }

    public async Task<ResponseDto> TrackKeywordsRankAsync(int? catId)
    {
        return await _decorated.TrackKeywordsRankAsync(catId);
    }

    public async Task SaveSeperResponseToMongo(List<SerpApiDto> response, string id, int crawlDateId,
        int KeywordGroupId)
    {
        await _decorated.SaveSeperResponseToMongo(response, id, crawlDateId, KeywordGroupId);
    }

    public async Task ProccessKeywordTracktion(int catId, int crawlDateId, List<Keyword> keywords, List<Site> sites)
    {
        await _decorated.ProccessKeywordTracktion(catId, crawlDateId, keywords, sites);
    }

    public async Task ProcessSiteRankAsync(Site site, SerpApiDto res, Keyword keyword, int crawlDateId)
    {
        await _decorated.ProcessSiteRankAsync(site, res, keyword, crawlDateId);
    }

    public async Task<List<SerpApiDto>> RequestKeywordRank(List<string> queries)
    {
        return await _decorated.RequestKeywordRank(queries);
    }

    public async Task<List<NightWatchDto>> GetAllKeywordsFromNightWatch()
    {
        return await _decorated.GetAllKeywordsFromNightWatch();
    }

    public async Task AddRank(BaseRankDto rank)
    {
        await _decorated.AddRank(rank);
    }

    public async Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsync(int catId, int pageSize, int pageIndex,
        string? rank, int? tagId)
    {
        string key = $"cat-{catId}-pageSize-{pageSize}-pageIndex-{pageIndex}-rank-{rank}-tagId-{tagId}";


        string? cache = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cache))
        {
            var response = await _decorated.GetRanksByCategoryAsync(catId, pageSize, pageIndex, rank, tagId);

            if (response is null)
            {
                return response;
            }

            var options = new DistributedCacheEntryOptions
            {
                // Set sliding expiration to 30 minutes
                SlidingExpiration = TimeSpan.FromMinutes(120)
            };

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(response), options);

            return response;

        }

        return JsonConvert.DeserializeObject<PaginitionResDto<RankDto>>(cache);

    }

    public async Task<PaginitionResDto<RankDto>> GetRanksByCategoryAsyncOptimized(int catId, int pageSize,
        int pageIndex, string? rank, int? tagId)
    {
        string key = $"cat-{catId}-pageSize-{pageSize}-pageIndex-{pageIndex}-rank-{rank}-tagId-{tagId}-optimized";


        string? cache = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cache))
        {
            var response = await _decorated.GetRanksByCategoryAsyncOptimized(catId, pageSize, pageIndex, rank, tagId);

            if (response is null)
            {
                return response;
            }

            var options = new DistributedCacheEntryOptions
            {
                // Set sliding expiration to 30 minutes
                SlidingExpiration = TimeSpan.FromHours(24)
            };

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(response), options);

            return response;
            
        }
        return JsonConvert.DeserializeObject<PaginitionResDto<RankDto>>(cache);
    }

    public async Task<string> GetRanksByCategoryForExcelExportAsync(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
       return await _decorated.GetRanksByCategoryForExcelExportAsync(catId, pageSize, pageIndex, rank, tagId);
    }

    public async Task<PaginitionResDto<RankDto>> GenerateHistoricalDataByCatIdAsync(int catId, int pageSize, int pageIndex)
    {
        return await _decorated.GenerateHistoricalDataByCatIdAsync(catId, pageSize, pageIndex);
    }

    public async Task<List<QueryDto>> GenerateQueryDto(List<Keyword> keywords, int catId)
    {
        return await _decorated.GenerateQueryDto(keywords, catId);
    }

    public async Task<List<QueryDto>> GenerateQueryDtoForExcelExport(List<Keyword> keywords, int catId)
    {
        return await _decorated.GenerateQueryDtoForExcelExport(keywords, catId);
    }

    public async Task<Tuple<List<Keyword>, int>> FindKeywordsForHistoricalData(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
        return await _decorated.FindKeywordsForHistoricalData(catId, pageSize, pageIndex, rank, tagId);
    }

    public async Task<Tuple<List<KeywordResponseDto>, int>> FindKeywordsForHistoricalDataOptimized(int catId, int pageSize, int pageIndex, string? rank, int? tagId)
    {
        return await _decorated.FindKeywordsForHistoricalDataOptimized(catId, pageSize, pageIndex, rank, tagId);
    }

    public async Task<List<Site>> FindSitesForHistoricalData(int catId)
    {
        return await _decorated.FindSitesForHistoricalData(catId);
    }

    public async Task<PaginitionResDto<List<KeywordGroupDto>>> GetAllKeywordGroupsAsync()
    {
        
        string key = $"AllKeywordGroups";


        string? cache = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cache))
        {
            var response = await _decorated.GetAllKeywordGroupsAsync();

            if (response is null)
            {
                return response;
            }

            var options = new DistributedCacheEntryOptions
            {
                // Set sliding expiration to 30 minutes
                SlidingExpiration = TimeSpan.FromHours(24)
            };

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(response), options);

            return response;
            
        }
        return JsonConvert.DeserializeObject<PaginitionResDto<List<KeywordGroupDto>>>(cache);
        
    }

    public async Task RequestGenerateKeywordGroupHistory(int catId, int crawlDateId)
    {
         await _decorated.RequestGenerateKeywordGroupHistory(catId,crawlDateId);
    }

    public async Task<KeywordGroupHistory> GenerateKeywordGroupHistory(List<QueryDto> query, int crawlDateId, int groupId)
    {
        return await _decorated.GenerateKeywordGroupHistory(query, crawlDateId, groupId);
    }

    public async Task<SingleResDto<List<SiteDto>>> GetKeywordSites(int keywordId)
    {
        return await _decorated.GetKeywordSites(keywordId);
    }

    public async Task<SingleResDto<KeywordHistoryDto>> GetKeywordDetailAsync(int id)
    {
        return await _decorated.GetKeywordDetailAsync(id);
    }

    public async Task<SingleResDto<KeywordDto>> GetKeywordAsync(int id)
    {
        return await _decorated.GetKeywordAsync(id);
    }

    public async Task<SingleResDto<List<KeywordDto>>> GetAllKeywordsAsync()
    {
        return await _decorated.GetAllKeywordsAsync();
    }

    public async Task AddVerticalsWithCompetitors(List<VerticalWithCompetitorsDto> verticalWithCompetitors)
    {
        await _decorated.AddVerticalsWithCompetitors(verticalWithCompetitors);
    }

    public async Task AddTag(List<TagDto> tags)
    {
        await _decorated.AddTag(tags);
    }

    public async Task AddKeywordsWithTags(List<KeywordWithTagsDto> keywordWithTags)
    {
        await _decorated.AddKeywordsWithTags(keywordWithTags);
    }

    public async Task AddTagToKeyword(KeywordTag keywordTag)
    {
        await _decorated.AddTagToKeyword(keywordTag);
    }

    public async Task<SingleResDto<List<TagReqDto>>> GetAllTags()
    {
        string key = $"AllTags";


        string? cache = await _distributedCache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cache))
        {
            var response = await _decorated.GetAllTags();

            if (response is null)
            {
                return response;
            }

            var options = new DistributedCacheEntryOptions
            {
                // Set sliding expiration to 30 minutes
                SlidingExpiration = TimeSpan.FromHours(24)
            };

            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(response), options);

            return response;
            
        }
        return JsonConvert.DeserializeObject<SingleResDto<List<TagReqDto>>>(cache);
        
    }

    public async Task<SingleResDto<List<TagReqDto>>> GetKeywordTagById(int keywordId)
    {
        return await _decorated.GetKeywordTagById(keywordId);
    }

    public async Task RemoveKeywordTag(int keywordId, int tagId)
    {
        await _decorated.RemoveKeywordTag(keywordId, tagId);
    }

    public async Task AddKeywordUrl(string link, int keywordId, int crawlDateId)
    {
        await _decorated.AddKeywordUrl(link, keywordId, crawlDateId);
    }

    public async Task GetKeywordsSearchVolume()
    {
        await _decorated.GetKeywordsSearchVolume();
    }

    public async Task AddKeywordsSerachVolume(KeywordToolDto data)
    {
        await _decorated.AddKeywordsSerachVolume(data);
    }

    public async Task<string> ExportKeywordsToExcel(RankDto rank, int catId)
    {
       return await _decorated.ExportKeywordsToExcel(rank, catId);
    }

    public async Task DownloadHtml(int catId)
    {
        await _decorated.DownloadHtml(catId);
    }

    public async Task SaveDownloadedHtml(KeywordHtml html)
    {
        await _decorated.SaveDownloadedHtml(html);
    }
}