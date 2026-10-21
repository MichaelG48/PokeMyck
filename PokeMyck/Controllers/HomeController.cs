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

        //[HttpGet]
        //public async Task<IActionResult> PokemonList(int pagina = 1)
        //{
        //    const int pokemonesPorPagina = 20;
        //    pagina = Math.Max(1, pagina);
        //    var offset = (pagina - 1) * pokemonesPorPagina;
        //    var (pokemones, totalPokemones) = await _pokeApiService.GetPokemonList(pokemonesPorPagina, offset);

        //    var _pokemon = new PokemonListPagination
        //    {
        //        Pokemones = pokemones,
        //        PaginaActual = pagina,
        //        PokemonesPorPagina = pokemonesPorPagina,
        //        TotalPokemones = totalPokemones
        //    };
        //    return View(_pokemon);
        //}

        [HttpGet]
        public async Task<IActionResult> PokemonList(
    int pagina = 1,
    string? NombreBuscado = null,
    string? EspecieSeleccionada = null)
        {
            const int pokemonesPorPagina = 20;

            pagina = Math.Max(1, pagina);

            var (pokemones, _) = await _pokeApiService.GetPokemonList(
                limit: 2000,
                offset: 0);

            var especies = await _pokeApiService.GetSpeciesList();

            if (!string.IsNullOrWhiteSpace(NombreBuscado))
            {
                pokemones = pokemones
                    .Where(p => p.Name.Contains(
                        NombreBuscado.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(EspecieSeleccionada))
            {
                var nombresEspecie =
                    await _pokeApiService.GetPokemonNamesBySpecies(
                        EspecieSeleccionada);

                pokemones = pokemones
                    .Where(p => nombresEspecie.Contains(
                        p.Name,
                        StringComparer.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalPokemones = pokemones.Count;

            var pokemonesPagina = pokemones
                .Skip((pagina - 1) * pokemonesPorPagina)
                .Take(pokemonesPorPagina)
                .ToList();

            var modelo = new PokemonListPagination
            {
                Pokemones = pokemonesPagina,
                PaginaActual = pagina,
                PokemonesPorPagina = pokemonesPorPagina,
                TotalPokemones = totalPokemones,
                NombreBuscado = NombreBuscado,
                EspecieSeleccionada = EspecieSeleccionada,
                Especies = especies
            };

            return View(modelo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
