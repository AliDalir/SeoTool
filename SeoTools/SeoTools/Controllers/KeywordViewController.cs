using BusinessLayer.Repositories;
using DataAccessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace SeoTools.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KeywordViewController : ControllerBase
{

    private readonly IViewService _viewService;

    public KeywordViewController(IViewService viewService)
    {
        _viewService = viewService;
    }


    [HttpGet("CreateView")]
    public async Task CreateView(string viewName)
    {
        await _viewService.CreateView(viewName);
    }
    
    [HttpGet("GetAllViews")]
    public async Task<List<ViewDto>> GetViews()
    {
        return await _viewService.GetAllViews();
    }
    
    [HttpPost("AddKeywordsToView")]
    public async Task AddKeywordsToView(KeywordViewCreateDto data)
    {
        await _viewService.AddKeywordsToView(data);
    }
}