using BusinessLayer.Repositories;
using Nest;

namespace BusinessLayer.Services;

public class ElasticService<T> : IElasticService<T> where T : class
{

    private readonly IElasticClient _elasticClient;

    public ElasticService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<string> CreateDocumentAsync(T document,string index)
    {
        var response = await _elasticClient.IndexAsync<T>(document, i => i.Index(index));


      return response.IsValid ? "Document created successfull" : "Failed to create document";
    }

    public async Task<string> CreateBulkDocumentAsync(IEnumerable<T> documents)
    {
        var response =  await _elasticClient.IndexManyAsync(documents);

        return response.IsValid ? "Documents created successfull" : "Failed to create documents";
    }

    public async Task<T> GetDocumentAsync(int id)
    {
        var response = await _elasticClient.GetAsync(new DocumentPath<T>(id));

        return response.Source;
    }

    public async Task<string> UpdateDocumentAsync(T document)
    {
        var response =
            await _elasticClient.UpdateAsync(new DocumentPath<T>(document), 
                u => u.Doc(document).RetryOnConflict(3));
        
        return response.IsValid ? "Document update successfull" : "Failed to update document";
    }

    public async Task<string> DeleteDocumentAsync(int id)
    {
        var response =  await _elasticClient.DeleteAsync(new DocumentPath<T>(id));

        return response.IsValid ? "Document deleted successfull" : "Failed to delete document";
    }
    
}