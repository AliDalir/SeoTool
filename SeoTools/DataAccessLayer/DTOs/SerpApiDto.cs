using Newtonsoft.Json;


public class SerpApiDto
{
    [JsonProperty("searchParameters")]
    public SearchParameters SearchParameters { get; set; }
    
    [JsonProperty("organic")] 
    public List<Organic> Organic { get; set; }
    
    [JsonProperty("relatedSearches")]
    public List<RelatedSearch> RelatedSearches { get; set; }
    
    [JsonProperty("answerBox")]
    public AnswerBox AnswerBox { get; set; }
}

public class SearchParameters
{
    public string q { get; set; }
}
public class Organic
{
    [JsonProperty("title")] 
    public string Title { get; set; }

    [JsonProperty("link")] 
    public string Link { get; set; }

    [JsonProperty("position")] 
    public int Position { get; set; }
}

public class AnswerBox
{
    [JsonProperty("snippet")]
    public string Snippet { get; set; }

    [JsonProperty("snippetHighlighted")]
    public string[] SnippetHighlighted { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("link")]
    public string Link { get; set; }
}

public class RelatedSearch
{
    [JsonProperty("query")]
    public string Query { get; set; }
}