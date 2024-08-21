using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchConsoleUrl
{
    [Key]
    public int Id { get; set; }

    public int SearchConsoleKeywordId { get; set; }

    public string Page { get; set; }


    public SearchConsoleKeyword SearchConsoleKeyword { get; set; }
}