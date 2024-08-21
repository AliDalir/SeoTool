using BusinessLayer.Repositories;
using Nest;

namespace BusinessLayer.Services;

public class SearchService<T> : ISearchService<T> where T : class
{
    
    private readonly IElasticClient _elasticClient;

    public SearchService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<IEnumerable<T>> SearchInElastic(string query, string index)
    {
        var response = await _elasticClient.SearchAsync<T>(s => s
            .Index(index)
            .Query(q => q.QueryString(d => d.Query(query)))
            .Size(10));

        if (!response.IsValid)
        {
            // Handle error
            throw new Exception(response.OriginalException.Message);
        }

        return response.Documents.ToList();
    }

}