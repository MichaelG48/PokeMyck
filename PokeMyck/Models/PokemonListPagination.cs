namespace PokeMyck.Models
{
    public class PokemonListPagination
    {
        public List<Pokemon> Pokemones { get; set; } = new();

        public int PaginaActual { get; set; } = 1;

        public int PokemonesPorPagina { get; set; } = 20;

        public int TotalPokemones { get; set; }

        public string? NombreBuscado { get; set; }

        public string? EspecieSeleccionada { get; set; }

        public List<EspeciesPokemon> Especies { get; set; } = new();

        public int TotalPaginas =>
            PokemonesPorPagina > 0
                ? (int)Math.Ceiling(
                    (double)TotalPokemones / PokemonesPorPagina)
                : 0;
    }
}
