using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController : ControllerBase
{

    private readonly IReportService _reportService;

    private readonly IGoogleSearchConsoleService _googleSearchConsoleService;
    public ReportController(IReportService reportService, IGoogleSearchConsoleService googleSearchConsoleService)
    {
        _reportService = reportService;
        _googleSearchConsoleService = googleSearchConsoleService;
    }


    [HttpGet("CreateReport")]
    public async Task<ReportDto> CreateReport(string keywordType, int keywordGroupId,int siteId,string rank ,int? tageId, int? searchVolume,bool branded)
    {
        var response =
            await _reportService.GetReport(keywordType, keywordGroupId, siteId,rank, tageId, searchVolume, branded);

        return response;
    }
    
    [HttpGet("GetDashboard")]
    public async Task<DashboardDto> GetDashboard()
    {
        var response =
            await _reportService.GenerateDashboardReport();

        return response;
    }
    
    [HttpGet("GetDailyReport")]
    [OutputCache(Duration = 3600)]
    public async Task<SearchConsoleDefalutDto> GetDailyReport(string period)
    {
        var response =
            await _reportService.GetDailyOverview(period);

        return response;
    }
    
    [HttpGet("GetFullTypeReport")]
    [OutputCache(Duration = 3600)]
    public async Task<SearchConsoleTypeReportDto> GetFullTypeReport(string period)
    {
        var response =
            await _googleSearchConsoleService.GetFullTypeReport(period);

        return response;
    }
    
    [HttpGet("CompareKeywords")]
    // [OutputCache(Duration = 3600)]
    public async Task<List<KeywordCompareDto>> CompareKeywords()
    {
        var response =
            await _reportService.CompareKeywordRank();

        return response;
    }
}