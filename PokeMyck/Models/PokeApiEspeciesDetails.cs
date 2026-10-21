using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeMyck.Models
{
    public class PokeApiEspeciesDetails
    {
        [JsonPropertyName("pokemon")]
        public List<JsonElement> Pokemon { get; set; } = new();
    }
}
