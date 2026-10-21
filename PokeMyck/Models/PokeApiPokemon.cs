using System.Text.Json.Serialization;

namespace PokeMyck.Models
{
    public class PokeApiPokemon
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
