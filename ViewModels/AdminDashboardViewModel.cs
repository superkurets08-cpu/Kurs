using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int UsersCount { get; init; }
        public int TournamentsCount { get; init; }
        public int TeamsCount { get; init; }
        public int PlayersCount { get; init; }
        public int MatchesCount { get; init; }
        public int HeroesCount { get; init; }
        public int ItemsCount { get; init; }
        public int AdminLogsCount { get; init; }
        public IReadOnlyList<AdminActionLog> RecentAdminActions { get; init; } = Array.Empty<AdminActionLog>();
    }
}
