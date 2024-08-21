using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchConsoleKeyword
{

    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Query { get; set; }
    
    

    public List<SearchConsoleUrl> SearchConsoleUrl { get; set; }

    public List<SearchConsoleKeywordRank> SearchConsoleKeywordRanks { get; set; }
}