using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class KeywordGroupHistory : BaseEntity
{
    public int CrawlDateId { get; set; }

    public int KeywordGroupId { get; set; }

    public double AllAvgPosition { get; set; }

    public double LastAvgPosition { get; set; }

    public int Top3Count { get; set; }

    public double Top3AvgPosition { get; set; }

    public int Top10Count { get; set; }

    public double Top10AvgPosition { get; set; }

    public int Top100Count { get; set; }

    public double Top100AvgPosition { get; set; }

    public int NoRankCount { get; set; }

    public int KeywordCount { get; set; }


    public KeywordGroup KeywordGroup { get; set; }
    public CrawlDate CrawlDate { get; set; }
}