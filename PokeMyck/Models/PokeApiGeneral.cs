using System.Text.Json.Serialization;

namespace PokeMyck.Models
{
    public class PokeApiGeneral
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<PokeApiPokemon> Results { get; set; } = new();
    }
}
