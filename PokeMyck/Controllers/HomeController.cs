using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PokeMyck.Models;
using PokeMyck.Services;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PokeMyck.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PokeApiService _pokeApiService;

        public HomeController(ILogger<HomeController> logger, PokeApiService pokeApiService)
        {
            _logger = logger;
            _pokeApiService = pokeApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //Get the list of pokemons with pagination

        [HttpGet]
        public async Task<IActionResult> PokemonList(int pagina = 1)
        {
            const int pokemonesPorPagina = 20;
            pagina = Math.Max(1, pagina);
            var offset = (pagina - 1) * pokemonesPorPagina;
            var (pokemones, totalPokemones) = await _pokeApiService.GetPokemonList(pokemonesPorPagina, offset);
            
            var _pokemon = new PokemonListPagination
            {
                Pokemones = pokemones,
                PaginaActual = pagina,
                PokemonesPorPagina = pokemonesPorPagina,
                TotalPokemones = totalPokemones
            };
            return View(_pokemon);
        }

        // Get the details of a specific pokemon by name
        [HttpGet]
        public async Task<IActionResult> PokemonDetails(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest();
            }

            try
            {
                var pokemon = await _pokeApiService.GetPokemonDetails(nombre);
                return View(pokemon);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
