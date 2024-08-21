using DataAccessLayer.Entites;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayer.MongoEntities;

public class RankTrackerResponse
{
    [BsonId]
    public int Id { get; set; }

    public DateTime DateTime { get; set; } = System.DateTime.Today;

    public int PageIndex { get; set; }

    public int PageSize { get; set; }
    
    public List<Keyword> Keywords { get; set; }
    
    
}