using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entites;

public class SearchConsoleReport
{
    [Key]
    public int Id { get; set; }

    public string Date { get; set; }
    
    public double Clicks { get; set; }

    public double Impressions { get; set; }

    public double Ctr { get; set; }

    public double AvgPosition { get; set; }
}