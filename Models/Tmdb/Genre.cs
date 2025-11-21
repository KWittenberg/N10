using System.Text.Json.Serialization;

namespace N10.Models.Tmdb;

public class Genre
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
