using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class TeamDetailsViewModel
    {
        public Team Team { get; init; } = new();
        public IReadOnlyList<Player> Players { get; init; } = Array.Empty<Player>();
        public IReadOnlyList<Match> Matches { get; init; } = Array.Empty<Match>();
        public IReadOnlyList<RosterMembership> RosterHistory { get; init; } = Array.Empty<RosterMembership>();
        public int WinsCount { get; init; }
    }
}
