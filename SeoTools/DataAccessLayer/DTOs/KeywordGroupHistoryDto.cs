namespace DataAccessLayer.DTOs;

public class KeywordGroupHistoryDto
{
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

    public CrawlDateDto CrawlDate { get; set; }
}