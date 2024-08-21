using DataAccessLayer.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace BusinessLayer.Elasticsearch;

public static class ElasticsearchExtensions
{
    public static void AddElasticSearch(this IServiceCollection service, IConfiguration configuration)
    {
        var url = configuration["ELKConfiguration:Uri"];
        var defaultIndex = configuration["ELKConfiguration:index"];
        var username = configuration["ELKConfiguration:Username"];
        var password = configuration["ELKConfiguration:Password"];

        var settings = new ConnectionSettings(new Uri(url))
            .BasicAuthentication(username, password) // Add this line
            .PrettyJson()
            .DefaultIndex(defaultIndex);


        
        AddDefaultMappings(settings);

        var client = new ElasticClient(settings);

        service.AddSingleton<IElasticClient>(client);
        
        CreateIndex(client,defaultIndex);
    }


    private static void AddDefaultMappings(ConnectionSettings settings)
    {
        settings.DefaultMappingFor<SearchConsoleData>(d => d);
    }

    private static void CreateIndex(IElasticClient client, string indexName)
    {
        client.Indices.Create(indexName, 
            i => i.Map<SearchConsoleData>(x
                => x.AutoMap()));
    }
}