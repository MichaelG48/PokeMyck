using System.Net.Http.Json;
using PokeMyck.Models;

namespace PokeMyck.Services
{
    public class PokeApiService
    {
        private readonly HttpClient _httpClient;

        public PokeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
                var id = pokemon.Url.TrimEnd('/').Split('/').Last();

                return new Pokemon
                {
                    Name = pokemon.Name,
                    Url = pokemon.Url,
                    ImageUrl = GetImageUrl(pokemon.Url)
                };
            }).ToList();

            return (pokemones, pokeApiResponse.Count);
        }

        private static string GetImageUrl(string pokemonUrl)
        {
            var segmento = pokemonUrl.TrimEnd('/')
                .Split('/')
                .Last();

            return $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{segmento}.png";
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

        public async Task<List<string>> GetPokemonNamesBySpecies(
            string especie)
        {
            var respuesta = await _httpClient.GetAsync(
                $"pokemon-species/{Uri.EscapeDataString(especie)}");

            respuesta.EnsureSuccessStatusCode();

            var contenido = await respuesta.Content.ReadFromJsonAsync<
                System.Text.Json.JsonElement>();

            var nombres = contenido.GetProperty("pokemon")
                .EnumerateArray()
                .Select(item => item.GetProperty("pokemon")
                    .GetProperty("name").GetString()!)
                .ToList();

            return nombres;
        }
    }
}
