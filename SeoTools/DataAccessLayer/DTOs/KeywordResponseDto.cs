using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Entites;

namespace DataAccessLayer.DTOs;

public class KeywordResponseDto
{
    public int KeywordGroupId { get; set; }
    
    public int KeywordId { get; set; }
    
    [Required] 
    public string Query { get; set; }
    
    public double CurrentRank { get; set; }

    public string? URL { get; set; }
    
    public string KeywordSearchVolume { get; set; }
    
    public List<TagReqDto> KeywordTags { get; set; }

    public List<CompetitorsRanks>? CompetitorsRank { get; set; }
}

public class CompetitorsRanks
{
    public int SiteId { get; set; }
    
    
    public double Position { get; set; }
}