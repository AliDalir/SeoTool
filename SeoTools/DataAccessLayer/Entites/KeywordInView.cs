namespace DataAccessLayer.Entites;

public class KeywordInView
{
    public int Id { get; set; }

    public int ViewId { get; set; }

    public int KeywordId { get; set; }



    public View Views { get; set; }
    public Keyword Keywords { get; set; }
}