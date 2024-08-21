namespace DataAccessLayer.DTOs;

public class KeywordHistoryDto
{
    public int KeywordGroupId { get; set; }

    public string Query { get; set; }


    public List<BaseRankDto> Ranks { get; set; }
    public KeywordGroupDto KeywordGroup { get; set; }

    public List<KeywordUrlDto> KeywordUrls { get; set; }
    public List<SiteDto> Sites { get; set; }
}