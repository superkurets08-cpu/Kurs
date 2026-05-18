namespace KURS_ASP.NET.ViewModels
{
    public class SearchViewModel
    {
        public string Query { get; init; } = string.Empty;
        public IReadOnlyList<SearchResultItemViewModel> Results { get; init; } = Array.Empty<SearchResultItemViewModel>();
        public IReadOnlyList<SearchCategoryGroupViewModel> Groups { get; init; } = Array.Empty<SearchCategoryGroupViewModel>();
        public IReadOnlyList<SearchSuggestionViewModel> Suggestions { get; init; } = Array.Empty<SearchSuggestionViewModel>();
        public int TotalCount => Results.Count;
    }
}
