using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchConsoleKeywordRank
{
    [Key]
    public int Id { get; set; }

    public int SearchConsoleKeywordId { get; set; }

    public int SearchConsoleDateId { get; set; }
    
    public string Clicks { get; set; }
    
    public string Ctr { get; set; }
    
    public string Impressions { get; set; }
    
    public string Position { get; set; }



    public SearchConsoleKeyword SearchConsoleKeyword { get; set; }

    public SearchConsoleDate SearchConsoleDate { get; set; }
}