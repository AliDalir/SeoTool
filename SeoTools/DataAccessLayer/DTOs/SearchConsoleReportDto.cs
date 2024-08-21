using DataAccessLayer.Entites;

namespace DataAccessLayer.DTOs;

public class SearchConsoleReportDto
{
    public double AllClick { get; set; }

    public double AllImpression { get; set; }

    public double AvgCtr { get; set; }

    public double AvgPosition { get; set; }
    
    public List<string> Lables { get; set; }

    public List<SearchConsoleReport> SearchConsoleReports { get; set; }
    
}