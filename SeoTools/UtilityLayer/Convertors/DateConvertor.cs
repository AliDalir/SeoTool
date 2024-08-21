using System.Globalization;
using MD.PersianDateTime;

namespace UtilityLayer.Convertors;

public static class DateConvertor
{
    public static DateTime ConvertShamsiToMiladi(string date)
    {
        var persianDateTime = PersianDateTime.Parse(date);
        return persianDateTime.ToDateTime();
    }

    public static string ConvertMiladiToShamsi(this DateTime? date, string format)
    {
        var persianDateTime = new PersianDateTime(date);
        return persianDateTime.ToString(format);
    }
    
    public static List<string> GetAllDatesOfLastMonth()
    {
        
        
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        
        // Get today's date
        DateTime today = DateTime.Today.AddDays(-60);

        // Find the first day of the current month
        DateTime firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);

        // Subtract one day to get the last day of the last month
        DateTime lastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);

        // Find the first day of the last month
        DateTime firstDayOfLastMonth = new DateTime(lastDayOfLastMonth.Year, lastDayOfLastMonth.Month, 1);

        // List to hold all dates of the last month
        List<string> datesOfLastMonth = new List<string>();

        // Generate all dates from the first to the last day of the last month
        for (DateTime date = firstDayOfLastMonth; date <= lastDayOfLastMonth; date = date.AddDays(1))
        {
            datesOfLastMonth.Add(date.ToString("yyyy-MM-dd 00:00:00", CultureInfo.InvariantCulture));
        }

        return datesOfLastMonth;
    }
    
    public static (string startDate, string endDate) GetDateRange(string period)
    {
        
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        
        DateTime baseEndDate = DateTime.Now.Date.AddDays(-3); // This is the end date for all calculations
        DateTime endDate = baseEndDate; // Default end date is 2 days ago from now
        DateTime startDate = endDate; 



        switch (period.ToLower())
        {
            case "latest":
                startDate = baseEndDate;
                break;
            case "last28days":
                startDate = endDate.AddDays(-28);
                break;
            case "last3months":
                startDate = endDate.AddMonths(-3);
                break;
            case "last6months":
                startDate = endDate.AddMonths(-6);
                break;
            case "last12months":
                startDate = endDate.AddMonths(-12);
                break;
            case "last16months":
                startDate = endDate.AddMonths(-16);
                break;
            case "lastmonth":
                // Set startDate to 56 days ago and endDate to 28 days ago
                startDate = baseEndDate.AddDays(-56);
                endDate = baseEndDate.AddDays(-28);
                break;
            default:
                throw new ArgumentException("Invalid period specified");
        }

        return (startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
    }
    
    public static List<string> GetAllDatesInRange(string startDateString, string endDateString)
    {
        var dates = new List<string>();

        if (!DateTime.TryParseExact(startDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
        {
            throw new ArgumentException("Start date is not in the correct format.", nameof(startDateString));
        }

        if (!DateTime.TryParseExact(endDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var endDate))
        {
            throw new ArgumentException("End date is not in the correct format.", nameof(endDateString));
        }

        if (endDate < startDate)
        {
            throw new ArgumentException("End date must be greater than or equal to the start date.");
        }

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            dates.Add(date.ToString("yyyy-MM-dd"));
        }

        return dates;
    }
}