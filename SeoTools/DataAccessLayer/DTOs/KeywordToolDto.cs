namespace DataAccessLayer.DTOs;

public class KeywordToolDto
{
    public Dictionary<string, KeywordData> Results { get; set; }
}

public class KeywordData
{
    public string String { get; set; }
    public int? Volume { get; set; }
    public double? Trend { get; set; }
    public double? Cpc { get; set; }
    public double? Cmp { get; set; }
}