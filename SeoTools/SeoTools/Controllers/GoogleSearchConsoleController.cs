using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SeoTools.Controllers;


[Route("api/[controller]")]
[ApiController]
// [Authorize]
public class GoogleSearchConsoleController : ControllerBase
{

    private readonly IGoogleSearchConsoleService _googleSearchConsole;
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IConfiguration _configuration;

    public GoogleSearchConsoleController(IGoogleSearchConsoleService googleSearchConsole, IBackgroundJobClient backgroundJobs, IConfiguration configuration)
    {
        _googleSearchConsole = googleSearchConsole;
        _backgroundJobs = backgroundJobs;
        _configuration = configuration;
    }


    [HttpGet("GetSearchData")]
    public async Task<string> GetSearchData(string startDate, string endDate)
    {
        
        string site = _configuration.GetSection("DomainName").ToString();
        string creds = _configuration.GetSection("creds").ToString();
        string output = "gsc_data.csv";
        
        
        await _googleSearchConsole.ExtractData(site, true, startDate, endDate, output);
        return "Your request have been added to queue";
    }
    
    [HttpGet("GetDataFromElastic")]
    public string GetDataFromElastic()
    {
        _backgroundJobs.Schedule(() => _googleSearchConsole.ReadGoogleSearchConsoleDataFromElastic(),
            DateTimeOffset.Now.AddMinutes(1));
        
        // await _googleSearchConsole.ReadGoogleSearchConsoleDataFromElastic();
        
        return "Your request have been added to queue";
    }
    
    [HttpGet("GetFullReport")]
    public async Task<string> GetFullReport(string period)
    {
        
        await _googleSearchConsole.GetFullReportOfSearchConsole(period);
        
        return "Your request have been added to queue";
    }
    
    [HttpGet("GetSearchConsoleReport")]
    public async Task<SearchConsoleReportDto> GetSearchConsoleReport(string period)
    {
        
        return await _googleSearchConsole.GetSearchConsoleReport(period);
    }
    
    [HttpGet("GetSearchConsoleKeywords")]
    public async Task<PaginitionResDto<List<SearchConsoleResponse>>>  GetSearchConsoleKeywords(int pageSize,int pageIndex)
    {
        return await _googleSearchConsole.GetSearchConsoleKeywords(pageSize,pageIndex);
    }
}