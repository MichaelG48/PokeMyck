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

        [HttpPost]
        public IActionResult DescargarPokemon(List<Pokemon> pokemones)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Nombre;URL de imagen");

            foreach (var pokemon in pokemones)
            {
                csv.AppendLine(
                    $"\"{pokemon.Name}\";\"{pokemon.ImageUrl}\"");
            }

            var contenido = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
                .ToArray();

            return File(
                contenido,
                "application/vnd.ms-excel",
                "Pokemon.xls");
        }

        [HttpPost]
        public async Task<IActionResult> SendByEmail(
            List<Pokemon> pokemones,
            string correoDestino)
        {
            if (pokemones == null || pokemones.Count == 0) {
                return BadRequest("No pokemons found");
            }
            if (string.IsNullOrWhiteSpace(correoDestino)) {
                return BadRequest("Email Invalid");
            }

            var cuerpo = new System.Text.StringBuilder();
            cuerpo.AppendLine("NAME | IMAGE URL");
            cuerpo.AppendLine();

            foreach (var pokemon in pokemones)
            {
                cuerpo.AppendLine($"{pokemon.Name} | {pokemon.ImageUrl}");
            }

            try
            {
                using var mensaje = new System.Net.Mail.MailMessage();
                mensaje.From = new System.Net.Mail.MailAddress(
                    "michael.gonzalez1205@gmail.com");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = "POKEMON LIST BY POKEAPI";
                mensaje.Body = cuerpo.ToString();

                using var smtp = new System.Net.Mail.SmtpClient(
                    "smtp.gmail.com", 587);

                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(
                    "EMAIL@GMAIL.COM",
                    "APP_PASSWORD");

                await smtp.SendMailAsync(mensaje);
            }
            catch (System.Net.Mail.SmtpException)
            {
                TempData["Error"] = "Don't send email. Check your email settings.";
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while sending the email.";
            }

            TempData["Mensaje"] = "Email sent successfully.";

            return RedirectToAction(nameof(PokemonList));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
