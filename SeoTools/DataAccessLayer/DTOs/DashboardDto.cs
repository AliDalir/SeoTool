namespace DataAccessLayer.DTOs;

public class DashboardDto
{
    #region Global

    public int GscKeywordCount { get; set; }

    public int RtKeywordCount { get; set; }
    
    public DateTime BestDayThisMonth { get; set; }

    public DateTime BestDayLastMonth { get; set; }

    public double BestClikThisMonth { get; set; }
    public double BestImpThisMonth { get; set; }

    public double BestClickLastMonth { get; set; }

    public double BestImpLastMonth { get; set; }
    
    public double DailyClick { get; set; }
    public double DailyImpression { get; set; }
    public double DailyCtr { get; set; }
    public double DailyAvgPosition { get; set; }
    public double DailyGroth { get; set; }
    public int DailyGrothPercentage { get; set; }

    #endregion

    #region Brand & None brand

    public int BrandedGscKeywordCount { get; set; }

    public int NoneBrandedGscKeywordCount { get; set; }
    
    public int BrandedRtKeywordCount { get; set; }

    public int NoneBrandedRtKeywordCount { get; set; }

    public int BrandedDailyClick { get; set; }

    public int NoneBrandedDailyClick { get; set; }

    public int BrandedDailyImpression { get; set; }

    public int NoneBrandedDailyImpression { get; set; }
    
    public int BrandedMonthlyClick { get; set; }

    public int NoneBrandedMonthlyClick { get; set; }

    public int BrandedMonthlyImpression { get; set; }

    public int NoneBrandedMonthlyImpression { get; set; }

    public int BrandedDailyCtr { get; set; }
    
    public int NoneBrandedDailyCtr { get; set; }
    
    public int BrandedMonthlyCtr { get; set; }
    
    public int NoneBrandedMonthlyCtr { get; set; }
    
    public int BrandedDailyAvgPosition { get; set; }
    
    public int NoneBrandedDailyAvgPosition { get; set; }
    
    public int BrandedMonthlyAvgPosition { get; set; }
    
    public int NoneBrandedMonthlyAvgPosition { get; set; }

    #endregion
    
    #region Trans & Info
    
    public int TransDailyClick { get; set; }
    
    public int InfoDailyClick { get; set; }
    
    public int TransDailyImpression { get; set; }
    
    public int InfoDailyImpression { get; set; }
    
    public int TransDailyCtr { get; set; }
    
    public int InfoDailyCtr { get; set; }
    
    public int TransDailyAvgPosition { get; set; }
    
    public int InfoDailyAvgPosition { get; set; }
    
    public int TransMonthlyClick { get; set; }
    
    public int InfoMonthlyClick { get; set; }
    
    public int TransMonthlyImpression { get; set; }
    
    public int InfoMonthlyImpression { get; set; }
    
    public int TransMonthlyCtr { get; set; }
    
    public int InfoMonthlyCtr { get; set; }
    
    public int TransMonthlyAvgPosition { get; set; }
    
    public int InfoMonthlyAvgPosition { get; set; }
    

    #endregion

    public List<CompetitorsSummeryDto> CompetitorsSummeries { get; set; }
    
    
    
    
    
    
}

public class CompetitorsSummeryDto
{
    public int SiteId { get; set; }

    public double AllAvgPosition { get; set; }

    public double LastAvgPosition { get; set; }

    public int Top3Count { get; set; }

    public double Top3AvgPosition { get; set; }

    public int Top10Count { get; set; }

    public double Top10AvgPosition { get; set; }

    public int Top100Count { get; set; }

    public double Top100AvgPosition { get; set; }

    public int NoRankCount { get; set; }

    public int KeywordCount { get; set; }
}