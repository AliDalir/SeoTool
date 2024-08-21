using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]

public class KeywordController : ControllerBase
{
    private readonly IKeywordService _keywordService;
    private readonly IHtmlService _htmlService;

    public KeywordController(IKeywordService keywordService, IHtmlService htmlService)
    {
        _keywordService = keywordService;
        _htmlService = htmlService;
    }


    [HttpGet("TrackKeywords")]
    public async Task<string> TrackKeywordsRank(int? catId)
    {
        // await _keywordService.CreateTrackRequest(catId);
        await _keywordService.TrackKeywordsRankAsync(catId);

        return "Your request have been added to queue";
    }
    
    //
    // [HttpPost("AddKeyword")]
    // [Authorize]
    // public async Task<ResponseDto> AddKeyword(KeywordDto keyword)
    // {
    //     return await _keywordService.AddKeywordAsync(keyword);
    // }

    [HttpGet("GetNightWatchKeywords")]
    [Authorize]
    public async Task<List<NightWatchDto>> GetNightWatchKeywords()
    {
        return await _keywordService.GetAllKeywordsFromNightWatch();
    }

    // [HttpGet("GetAllRanks")]
    // public async Task<PaginitionResDto<RankDto>> GetAllRanks(int pageSize,int pageIndex)
    // {
    //     return await _keywordService.GetAllRanks(pageSize,pageIndex);
    // }

    [HttpGet("GetRanksByCatId")]
    [OutputCache(Duration = 3600,VaryByQueryKeys = new string[] { "catId", "pageSize", "pageIndex","rank","tagId" })]
    public async Task<PaginitionResDto<RankDto>> GetRanksByCatId(int catId, int pageSize, int pageIndex,string? rank, int? tagId)
    {
        return await _keywordService.GetRanksByCategoryAsyncOptimized(catId, pageSize, pageIndex,rank,tagId);
    }
    
    
    [HttpGet("ExportExcel")]
    public async Task<string> ExportExcel(int catId, int keywordCount)
    {
         return await _keywordService.GetRanksByCategoryForExcelExportAsync(catId, keywordCount,1,null,null);
    }

    
    [HttpGet("GetAllKeywordGroups")]
    [Authorize]
    [OutputCache(Duration = 3600 * 6)]
    public async Task<PaginitionResDto<List<KeywordGroupDto>>> GetAllKeywordGroups()
    {
        return await _keywordService.GetAllKeywordGroupsAsync();
    }

    [HttpGet("GetKeywordSites")]
    [Authorize]
    [OutputCache(Duration = 3600 * 2,VaryByQueryKeys = new string[] { "keywordId" })]
    public async Task<SingleResDto<List<SiteDto>>> GetKeywordSites(int keywordId)
    {
        return await _keywordService.GetKeywordSites(keywordId);
    }

    [HttpGet("GetKeyword")]
    [Authorize]
    [OutputCache(Duration = 3600,VaryByQueryKeys = new string[] { "id" })]
    public async Task<SingleResDto<KeywordDto>> GetKeyword(int id)
    {
        return await _keywordService.GetKeywordAsync(id);
    }

    [HttpGet("GetKeywordDetail")]
    [Authorize]
    [OutputCache(Duration = 3600,VaryByQueryKeys = new string[] { "id" })]
    public async Task<SingleResDto<KeywordHistoryDto>> GetKeywordDetail(int id)
    {
        return await _keywordService.GetKeywordDetailAsync(id);
    }
    
    [HttpGet("GetAllKeywords")]
    [Authorize]
    [OutputCache(Duration = 3600)]
    public async Task<SingleResDto<List<KeywordDto>>> GetAllKeywords()
    {
        return await _keywordService.GetAllKeywordsAsync();
    }
    
    [HttpGet("GetAllTags")]
    [Authorize]
    [OutputCache(Duration = 3600)]
    public async Task<SingleResDto<List<TagReqDto>>> GetAllTags()
    {
        return await _keywordService.GetAllTags();
    }
    
    [HttpGet("AddTagToKeyword")]
    [Authorize]
    public async Task AddTagToKeyword(int keywordId,int tagId)
    {
        await _keywordService.AddTagToKeyword(new KeywordTag()
        {
            KeywordId = keywordId,
            TagId = tagId,
        });
    }
    
    [HttpGet("GetKeywordTags")]
    [Authorize]
    public async Task<SingleResDto<List<TagReqDto>>> GetKeywordTags(int keywordId)
    {
        return await _keywordService.GetKeywordTagById(keywordId);
    }
    
    [HttpGet("RemoveKeywordTag")]
    [Authorize]
    public async Task RemoveKeywordTag(int keywordId,int tagId)
    {
         await _keywordService.RemoveKeywordTag(keywordId,tagId);
    }
    
    [HttpGet("GetKeywordResearchSv")]
    public async Task GetKeywordResearchSv()
    {
        await _keywordService.GetKeywordsSearchVolume();
    }
    
    [HttpGet("DowloadHtml")]
    public async Task DowloadHtml(int catId)
    {
        await _keywordService.DownloadHtml(catId);
    }
    
    [HttpGet("GenerateHistoricalData")]
    public async Task GenerateHistoricalData(int catId,int crawlDateId)
    {
        await _keywordService.RequestGenerateKeywordGroupHistory(catId,crawlDateId);
    }
}