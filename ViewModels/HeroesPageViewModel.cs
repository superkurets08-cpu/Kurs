using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class HeroesPageViewModel
    {
        public IReadOnlyList<Hero> Heroes { get; init; } = Array.Empty<Hero>();
        public IReadOnlyList<string> Attributes { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> AttackTypes { get; init; } = Array.Empty<string>();
        public string SearchTerm { get; init; } = string.Empty;
        public string Attribute { get; init; } = string.Empty;
        public string AttackType { get; init; } = string.Empty;
        public int TotalCount { get; init; }
    }
}
