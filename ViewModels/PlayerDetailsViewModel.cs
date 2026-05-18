using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class PlayerDetailsViewModel
    {
        public Player Player { get; init; } = new();
        public Team? Team { get; init; }
        public IReadOnlyList<Match> RecentMatches { get; init; } = Array.Empty<Match>();
        public IReadOnlyList<RosterMembership> RosterHistory { get; init; } = Array.Empty<RosterMembership>();
        public int MvpAwardsCount { get; init; }
    }
}
