namespace UtilityLayer.GoogleSheets;

public class KeywordWithTagMapper
{
    public static List<KeywordWithTagsDto> MapFromRangeData(IList<IList<object>> values)
    {
        var items = new List<KeywordWithTagsDto>();
        foreach (var value in values.Skip(1))
        {
            
                        
            var text = value[1].ToString();
            
            text = text.Replace(" ","");

            var index = text.IndexOf(",");
            
            KeywordWithTagsDto item = new()
            {
                Keyword = value[0].ToString(),
                TagTitle = text.Substring(index + 1),
                KeywordGroupTitle = text.Substring(0,index)
            };
            items.Add(item);
        }
            
        

        
        
        return items;
    }
    public static IList<IList<object>> MapToRangeData(KeywordWithTagsDto item)
    {
        var objectList = new List<object>() { item.KeywordGroupTitle,item.TagTitle, item.Keyword };
        var rangeData = new List<IList<object>> { objectList };
        return rangeData;
    }
}