using DataAccessLayer.Base;

namespace DataAccessLayer.Entites;

public class KeywordTag : BaseEntity
{
    public int KeywordId { get; set; }
    
    public int TagId { get; set; }



    public Keyword Keyword { get; set; }
    public Tag Tag { get; set; }
}