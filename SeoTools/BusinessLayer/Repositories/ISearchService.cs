namespace BusinessLayer.Repositories;

public interface ISearchService <T>
{
    public Task<IEnumerable<T>> SearchInElastic(string query, string index);
}