using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class ItemsPageViewModel
    {
        public IReadOnlyList<Item> Items { get; init; } = Array.Empty<Item>();
        public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
        public string SearchTerm { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public bool NeutralOnly { get; init; }
        public int TotalCount { get; init; }
    }
}
