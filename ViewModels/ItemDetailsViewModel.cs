using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class ItemDetailsViewModel
    {
        public Item Item { get; init; } = new();
        public IReadOnlyList<Item> RelatedItems { get; init; } = Array.Empty<Item>();
        public IReadOnlyList<Hero> SuggestedHeroes { get; init; } = Array.Empty<Hero>();
        public string Summary { get; init; } = string.Empty;
        public string TimingAdvice { get; init; } = string.Empty;
    }
}
