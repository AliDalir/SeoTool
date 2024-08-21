using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class Site : BaseEntity
{
    public int KeywordGroupId { get; set; }
    public string SiteUrl { get; set; }

    public string SiteName { get; set; }


    public List<Rank> Ranks { get; set; }
    public KeywordGroup KeywordGroup { get; set; }
    public List<CompetitorsSummery> CompetitorsSummeries { get; set; }
}