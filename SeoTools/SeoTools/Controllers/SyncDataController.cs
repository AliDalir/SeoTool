using BusinessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SyncDataController : Controller
{

    private readonly ISyncDataService _syncDataService;

    public SyncDataController(ISyncDataService syncDataService)
    {
        _syncDataService = syncDataService;
    }


    [HttpGet("SyncKeywordWithElastic")]
    public async Task<string> SyncKeywordWithElastic()
    {
        await _syncDataService.SyncKeywordsWithElasticAsync();

        return "Your request have been done";
    }
}