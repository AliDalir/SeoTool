namespace UtilityLayer.GoogleSheets;

public class NmvGeneratorMapper
{
    public static List<NmvGeneratorDto> MapFromRangeData(IList<IList<object>> values)
    {
        var items = new List<NmvGeneratorDto>();
        foreach (var value in values.Skip(1))
        {

                NmvGeneratorDto item = new()
                {
                    Keyword = value[0].ToString()
                };
                items.Add(item);
            }
            
        

        
        
        return items;
    }
    public static IList<IList<object>> MapToRangeData(NmvGeneratorDto item)
    {
        var objectList = new List<object>() { item.Keyword,item.Tag };
        var rangeData = new List<IList<object>> { objectList };
        return rangeData;
    }
}