using DataAccessLayer.Entites;
using Microsoft.IdentityModel.Tokens;

namespace DataAccessLayer.DTOs;

public class ReportDto
{
    public string Id { get; set; }
    
    public int TotalCount { get; set; }
    
    public object Avg { get; set; }

    public DateTime CrawlDate { get; set; }
}
public class ReportQueryDto
{
    public int KeywordId { get; set; }

    public string Query { get; set; }

    public object CurrentPosition { get; set; }

    public object AvgPosition { get; set; }

    public string LastTrackTime { get; set; }

    public List<BaseRankDto> Ranks { get; set; }
    public List<KeywordUrlDto> KeywordUrls { get; set; }
    
    public List<Tag> Tags { get; set; }

    public List<KeywordSearchVolume> SearchVolumes { get; set; }
}