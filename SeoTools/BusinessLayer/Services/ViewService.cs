using AutoMapper;
using BusinessLayer.Repositories;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace BusinessLayer.Services;

public class ViewService : IViewService
{
    private readonly SeoToolDbContext _context;
    private readonly IDistributedCache _distributedCache;
    private readonly IMapper _mapper;

    public ViewService(SeoToolDbContext context, IDistributedCache distributedCache, IMapper mapper)
    {
        _context = context;
        _distributedCache = distributedCache;
        _mapper = mapper;
    }


    public async Task CreateView(string viewName)
    {
        await _context.Views.AddAsync(new View()
        {
            ViewName = viewName,
        });

        await _context.SaveChangesAsync();
    }

    public async Task AddKeywordsToView(KeywordViewCreateDto data)
    {
        List<KeywordInView> keywordsInView = new List<KeywordInView>();

        foreach (var keyword in data.KeywordsId)
        {
            if (!_context.KeywordInViews.Any(k => k.ViewId == data.ViewId && k.KeywordId == keyword))
            {
                keywordsInView.Add(new KeywordInView(){KeywordId = keyword,ViewId = data.ViewId});
            }
        }

        await _context.AddRangeAsync(keywordsInView);

        await _context.SaveChangesAsync();

    }

    public async Task<List<ViewDto>> GetAllViews()
    {
        var views = await _context.Views.ToListAsync();

        return _mapper.Map<List<ViewDto>>(views);
    }
}