using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class HeroDetailsViewModel
    {
        public Hero Hero { get; init; } = new();
        public IReadOnlyList<Hero> SimilarHeroes { get; init; } = Array.Empty<Hero>();
        public IReadOnlyList<Item> SuggestedItems { get; init; } = Array.Empty<Item>();
        public string Summary { get; init; } = string.Empty;
        public string Playstyle { get; init; } = string.Empty;
    }
}
