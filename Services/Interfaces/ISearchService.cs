using KURS_ASP.NET.ViewModels;

namespace KURS_ASP.NET.Services.Interfaces
{
    public interface ISearchService
    {
        Task<SearchViewModel> SearchAsync(string? query, bool includeRestrictedContent);
        Task<IReadOnlyList<SearchSuggestionViewModel>> GetSuggestionsAsync(string? query, bool includeRestrictedContent);
    }
}
