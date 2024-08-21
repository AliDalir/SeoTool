namespace BusinessLayer.Repositories;

public interface IElasticService<T>
{
    public Task<string> CreateDocumentAsync(T document,string index);
    
    public Task<string> CreateBulkDocumentAsync(IEnumerable<T> documents);

    public Task<T> GetDocumentAsync(int id);
    
    public Task<string> UpdateDocumentAsync(T document);

    public Task<string> DeleteDocumentAsync(int id);
    
    
}