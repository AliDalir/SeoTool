using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class View
{
    [Key]
    public int Id { get; set; }

    public string ViewName { get; set; }


    public List<KeywordInView> KeywordInViews { get; set; }
}