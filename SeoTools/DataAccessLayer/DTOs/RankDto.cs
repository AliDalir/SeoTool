using DataAccessLayer.Entites;

namespace DataAccessLayer.DTOs;

public class RankDto
{
    public List<KeywordGroupHistoryDto> KeywordGroupHistory { get; set; }
    public IList<SiteDto>? Sites { get; set; }

    public IList<KeywordResponseDto> Queries { get; set; }
}

public class QueryDto
{
    public int KeywordId { get; set; }

    public string Query { get; set; }

    public object CurrentPosition { get; set; }

    public object AvgPosition { get; set; }

    public string LastTrackTime { get; set; }

    public string SearchVolume { get; set; }

    public List<BaseRankDto> Ranks { get; set; }
    public List<KeywordUrlDto>? KeywordUrls { get; set; }
    
    public List<Tag>? Tags { get; set; }
}

public class SiteDto
{
    public int Id { get; set; }

    public int KeywordGroupId { get; set; }

    public string SiteUrl { get; set; }

    public string SiteName { get; set; }
}

public class SiteReqDto
{

    public int KeywordGroupId { get; set; }

    public string SiteUrl { get; set; }

    public string SiteName { get; set; }
}