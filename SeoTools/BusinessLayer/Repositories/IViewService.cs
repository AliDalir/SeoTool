using DataAccessLayer.DTOs;
using DataAccessLayer.Entites;

namespace BusinessLayer.Repositories;

public interface IViewService
{
    public Task CreateView(string viewName);

    public Task AddKeywordsToView(KeywordViewCreateDto data);

    public Task<List<ViewDto>> GetAllViews();
}