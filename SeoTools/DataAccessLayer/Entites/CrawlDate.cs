using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class CrawlDate
{
    [Key] public int Id { get; set; }

    public DateTime CrawlDateTime { get; set; }


    public List<Rank> Ranks { get; set; }
    public List<KeywordGroupHistory> KeywordGroupHistories { get; set; }
    public List<KeywordUrl> KeywordUrls { get; set; }
}