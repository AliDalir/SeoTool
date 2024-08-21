using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchConsoleDate
{
    [Key]
    public int Id { get; set; }

    public string Date { get; set; }



    public List<SearchConsoleKeywordRank> SearchConsoleKeywordRanks { get; set; }
}