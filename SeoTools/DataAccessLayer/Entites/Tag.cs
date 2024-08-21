using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class Tag : BaseEntity
{
    public string TagName { get; set; }


    public List<KeywordTag> KeywordTags { get; set; }
}