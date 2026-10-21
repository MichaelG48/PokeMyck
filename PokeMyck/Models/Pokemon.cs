namespace PokeMyck.Models
{
    public class Pokemon
    {
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Height { get; set; }
        public int Weight { get; set; }
        public List<string> Types { get; set; } = new();
        public List<string> Abilities { get; set; } = new();
    }
}
