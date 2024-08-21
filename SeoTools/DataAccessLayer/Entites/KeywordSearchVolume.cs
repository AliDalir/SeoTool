using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class KeywordSearchVolume
{
    [Key]
    public int Id { get; set; }

    public int KeywordId { get; set; }

    public string SearchVolume { get; set; }


    public Keyword Keyword { get; set; }
}