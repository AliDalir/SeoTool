namespace UtilityLayer.GoogleSheets;

public class VerticalWithCompetitorsMapper
{
    public static List<VerticalWithCompetitorsDto> MapFromRangeData(IList<IList<object>> values)
    {
        var items = new List<VerticalWithCompetitorsDto>();
        foreach (var value in values.Skip(1))
        {

            VerticalWithCompetitorsDto item = new()
            {
                 VerticalTitle = value[0].ToString(),
                 CompetitorUrl = value[1].ToString().Replace("https://","")
            };
            items.Add(item);
        }
            
        

        
        
        return items;
    }
    public static IList<IList<object>> MapToRangeData(VerticalWithCompetitorsDto item)
    {
        var objectList = new List<object>() { item.VerticalTitle,item.CompetitorUrl };
        var rangeData = new List<IList<object>> { objectList };
        return rangeData;
    }
}