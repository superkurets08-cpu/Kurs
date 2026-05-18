using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class PlayersPageViewModel
    {
        public IReadOnlyList<Player> Players { get; init; } = Array.Empty<Player>();
        public IReadOnlyList<string> AvailableCountries { get; init; } = Array.Empty<string>();
        public IReadOnlyList<string> AvailableRoles { get; init; } = Array.Empty<string>();
        public string SearchTerm { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public int TotalPlayers { get; init; }
        public int TotalTeams { get; init; }
        public int TotalCountries { get; init; }
    }
}
