using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.MongoEntities;

public class SerperResponse
{
    [BsonId]
    public string? Id { get; set; }
    
    public int CrawlDateId { get; set; }
    
    public int KeywordGroupId { get; set; }

    public List<SerpApiDto> Responses { get; set; }
}