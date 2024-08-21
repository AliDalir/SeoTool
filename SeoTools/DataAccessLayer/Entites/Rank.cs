using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class Rank : BaseEntity
{
    public int KeywordId { get; set; }

    public int CrawlDateId { get; set; }

    public int SiteId { get; set; }

    public int Position { get; set; }

    public string Location { get; set; }


    public Keyword Keyword { get; set; }

    public Site Site { get; set; }

    public CrawlDate CrawlDate { get; set; }
}