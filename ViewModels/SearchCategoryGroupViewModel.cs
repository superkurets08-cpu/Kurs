namespace KURS_ASP.NET.ViewModels
{
    public class SearchCategoryGroupViewModel
    {
        public string Category { get; init; } = string.Empty;
        public IReadOnlyList<SearchResultItemViewModel> Items { get; init; } = Array.Empty<SearchResultItemViewModel>();
    }
}
