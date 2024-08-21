using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class Keyword : BaseEntity
{
    public int KeywordGroupId { get; set; }

    [Required] public string Query { get; set; }


    public List<Rank> Ranks { get; set; }
    public KeywordGroup KeywordGroup { get; set; }

    public List<KeywordUrl> KeywordUrls { get; set; }

    public List<KeywordTag> KeywordTags { get; set; }
    
    public KeywordSearchVolume KeywordSearchVolume { get; set; }

    public List<KeywordInView> KeywordInView { get; set; }
}