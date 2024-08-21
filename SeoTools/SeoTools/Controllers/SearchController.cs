using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{

    private readonly ISearchService<KeywordELK> _searchService;

    public SearchController(ISearchService<KeywordELK> searchService)
    {
        _searchService = searchService;
    }


    [HttpGet("SearchKeyword")]
    public async Task<IEnumerable<KeywordELK>> SearchKeyword(string q)
    {
       var response = await _searchService.SearchInElastic(q,"keywords");

       return response;
    }
    
}