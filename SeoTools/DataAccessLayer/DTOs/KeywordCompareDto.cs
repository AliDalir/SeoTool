namespace DataAccessLayer.DTOs;

public class KeywordCompareDto
{
    public int KeywordId { get; set; }

    public string Query { get; set; }

    public int CurrentPosition { get; set; }

    public int PrevRank { get; set; }

    public double RankChange { get; set; }
}