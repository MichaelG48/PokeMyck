using Microsoft.AspNetCore.Mvc;
using PokeMyck.Models;
using System.Diagnostics;
using PokeMyck.Services;

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
