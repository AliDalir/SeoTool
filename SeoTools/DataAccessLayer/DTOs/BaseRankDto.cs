namespace DataAccessLayer.DTOs;

public class BaseRankDto
{
    public int KeywordId { get; set; }

    public int CrawlDateId { get; set; }
    public int SiteId { get; set; }

    public int Position { get; set; }

    public string Location { get; set; }

    public string CreationDateTime { get; set; }
}