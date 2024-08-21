namespace BusinessLayer.Repositories;

public interface ISyncDataService
{
    public Task SyncKeywordsWithElasticAsync();
}