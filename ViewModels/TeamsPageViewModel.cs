using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class TeamsPageViewModel
    {
        public IReadOnlyList<Team> Teams { get; init; } = Array.Empty<Team>();
        public IReadOnlyList<string> AvailableCountries { get; init; } = Array.Empty<string>();
        public string SearchTerm { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public int TotalCount { get; init; }
    }
}
