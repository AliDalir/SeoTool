using DataAccessLayer.Base;

namespace DataAccessLayer.DTOs;

public class DashboardReport : BaseEntity
{
    public int TotalRankTrackerKeywords { get; set; }

    public int TotalSearchConsoleUniqueKeywords { get; set; }

    public double TotalClicks { get; set; }

    public double TotalImperssion { get; set; }

    public double LastMonthTotalClicks { get; set; }

    public double LastMonthTotalImperssion { get; set; }

    public double AllTimeAvgCtr { get; set; }
    
    public double AllTimeAvgPosition { get; set; }
}