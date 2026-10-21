using PokeMyck.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PokeMyck.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;

        // Constructor that takes an HttpClient as a parameter
        public PokeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get a list of Pokemon from the PokeAPI with pagination support
        public async Task<(List<Pokemon> Pokemones, int TotalPokemones)> GetPokemonList(
            int limit = 20,
            int offset = 0)
        {
            if (limit < 1 || offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "No limit and offset.");
            }

            var pokeApiResponse = await _httpClient.GetFromJsonAsync<PokeApiGeneral>(
                $"pokemon/?limit={limit}&offset={offset}");

            if (pokeApiResponse is null)
            {
                throw new HttpRequestException(
                    "No results found.");
            }

            var pokemones = pokeApiResponse.Results.Select(pokemon => {
                return new Pokemon
                {
                    Name = pokemon.Name,
                    Url = pokemon.Url,
                    ImageUrl = GetImageUrl(pokemon.Url)
                };
            }).ToList();

            return (pokemones, pokeApiResponse.Count);
        }

        // get the details of a specific Pokemon by its name
        public async Task<Pokemon> GetPokemonDetails(string nombre)
        {
            var pokeApiResponse = await _httpClient.GetFromJsonAsync<JsonElement>(
                $"pokemon/{Uri.EscapeDataString(nombre)}");

            if (pokeApiResponse.ValueKind == JsonValueKind.Undefined)
            {
                throw new HttpRequestException(
                    "Pokemon not found.");
            }

            return new Pokemon
            {
                Name = pokeApiResponse.GetProperty("name").GetString(),
                Height = pokeApiResponse.GetProperty("height").GetInt32(),
                Weight = pokeApiResponse.GetProperty("weight").GetInt32(),

                Types = pokeApiResponse.GetProperty("types")
                    .EnumerateArray()
                    .Select(t => t.GetProperty("type")
                        .GetProperty("name").GetString()!)
                    .ToList(),

                Abilities = pokeApiResponse.GetProperty("abilities")
                    .EnumerateArray()
                    .Select(a => a.GetProperty("ability")
                        .GetProperty("name").GetString()!)
                    .ToList(),

                ImageUrl = pokeApiResponse.GetProperty("sprites")
                    .GetProperty("other")
                    .GetProperty("official-artwork")
                    .GetProperty("front_default")
                    .GetString() ?? string.Empty
            };
        }

        public async Task<List<EspeciesPokemon>> GetSpeciesList()
        {
            var respuesta = await _httpClient.GetFromJsonAsync<PokeApiGeneral>(
                "pokemon-species/?limit=2000");

            if (respuesta is null)
            {
                throw new HttpRequestException(
                    "Expecies not found.");
            }

            return respuesta.Results.Select(especie => new EspeciesPokemon
            {
                Name = especie.Name,
                Url = especie.Url
            }).ToList();
        }

        public async Task<List<string>> GetPokemonNamesBySpecies(string especie)
        {
            using var respuesta = await _httpClient.GetAsync(
                $"pokemon-species/{Uri.EscapeDataString(especie.Trim().ToLower())}");

            respuesta.EnsureSuccessStatusCode();

            using var documento = await respuesta.Content.ReadAsStreamAsync();
            using var json = await System.Text.Json.JsonDocument.ParseAsync(documento);

            var raiz = json.RootElement;

            if (!raiz.TryGetProperty("varieties", out var variedades))
            {
                throw new HttpRequestException(
                    $"Variations not found for species '{especie}'.");
            }

            var nombres = variedades
                .EnumerateArray()
                .Select(item => item.GetProperty("pokemon")
                    .GetProperty("name").GetString())
                .Where(nombre => nombre != null)
                .Select(nombre => nombre!)
                .ToList();

            return nombres;
        }


        // obtain the image URL for a Pokemon based on its API URL
        private static string GetImageUrl(string pokemonUrl)
        {
            var segmento = pokemonUrl.TrimEnd('/')
                .Split('/')
                .Last();

            return $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{segmento}.png";
        }
    }
}
