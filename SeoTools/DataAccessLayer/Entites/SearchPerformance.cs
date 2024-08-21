using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchPerformance
{
    [Key]
    public int Id { get; set; }

    public int SearchKeywordId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public int Clicks { get; set; }
    public int Impressions { get; set; }
    public float CTR { get; set; }
    public float Position { get; set; }

    [MaxLength(5000)]
    public string Page { get; set; }



    public SearchKeyword SearchKeyword { get; set; }
}