using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class KeywordsAverageRank
{
    [Key]
    public int Id { get; set; }

    public int KeywordId { get; set; }

    public double AvgPosition { get; set; }
}