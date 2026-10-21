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

        public async Task<List<Pokemon>> GetPokemonList(
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

            return pokeApiResponse.Results.Select(pokemon => new Pokemon
            {
                Name = pokemon.Name,
                Url = pokemon.Url,
                ImageUrl = GetImageUrl(pokemon.Url)
            }).ToList();
        }

        private static string GetImageUrl(string pokemonUrl)
        {
            var segmento = pokemonUrl.TrimEnd('/')
                .Split('/')
                .Last();

            return $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/{segmento}.png";
        }
    }
}
