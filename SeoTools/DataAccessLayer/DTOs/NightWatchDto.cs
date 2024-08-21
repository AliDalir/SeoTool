using Newtonsoft.Json;

namespace DataAccessLayer.Entites;

public class NightWatchDto
{
    [JsonProperty("query")] public string Query { get; set; }

    [JsonProperty("position")] public long? Position { get; set; }
}