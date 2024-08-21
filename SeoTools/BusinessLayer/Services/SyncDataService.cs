using AutoMapper;
using BusinessLayer.Repositories;
using DataAccessLayer.Context;
using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services;

public class SyncDataService : ISyncDataService
{
    
    private readonly SeoToolDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<SyncDataService> _logger;
    private readonly IElasticService<KeywordELK> _keywordElasticService;

    public SyncDataService(SeoToolDbContext context, IMapper mapper, ILogger<SyncDataService> logger, IElasticService<KeywordELK> keywordElasticService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _keywordElasticService = keywordElasticService;
    }

    public async Task SyncKeywordsWithElasticAsync()
    {
        var keywords = await _context.Keywords.ToListAsync();

        var mappedKeywords = _mapper.Map<IEnumerable<KeywordELK>>(keywords);

        var itemCount = mappedKeywords.Count();

        for (int i = 0; i <= itemCount; i = i + 1000)
        {
            await _keywordElasticService.CreateBulkDocumentAsync(mappedKeywords);
        }

        
    }
}