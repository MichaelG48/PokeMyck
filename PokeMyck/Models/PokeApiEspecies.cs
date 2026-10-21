using System.Text.Json.Serialization;

namespace PokeMyck.Models
{
    public class PokeApiEspecies
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public List<EspeciesPokemon> Results { get; set; } = new();
    }
}
