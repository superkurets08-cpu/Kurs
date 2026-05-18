using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class TournamentsPageViewModel
    {
        public IReadOnlyList<Tournament> Tournaments { get; init; } = Array.Empty<Tournament>();
        public IReadOnlyList<int> AvailableYears { get; init; } = Array.Empty<int>();
        public string SearchTerm { get; init; } = string.Empty;
        public int? Year { get; init; }
        public int TotalCount { get; init; }
    }
}
