using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class KeywordHtml
{
    [Key]
    public int Id { get; set; }

    public int KeywordId { get; set; }

    public int SiteId { get; set; }

    public int CrawlDateId { get; set; }
    
    public string HtmlUrl { get; set; }
    
}