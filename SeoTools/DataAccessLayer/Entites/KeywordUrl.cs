using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class KeywordUrl : BaseEntity
{
    public int KeywordId { get; set; }
    
    public int CrawlDateId { get; set; }
    
    public string Url { get; set; }
    


    public Keyword Keyword { get; set; }
    public CrawlDate CrawlDate { get; set; }
}