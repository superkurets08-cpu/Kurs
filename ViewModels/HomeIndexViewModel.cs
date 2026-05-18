using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<Tournament> RecentTournaments { get; init; } = Array.Empty<Tournament>();
        public int TournamentsCount { get; init; }
        public int TeamsCount { get; init; }
        public int PlayersCount { get; init; }
        public int HeroesCount { get; init; }
    }
}
