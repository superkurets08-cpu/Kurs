using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class AdminMatchFormViewModel
    {
        public Match Match { get; init; } = new();
        public IReadOnlyList<Tournament> Tournaments { get; init; } = Array.Empty<Tournament>();
        public IReadOnlyList<Team> Teams { get; init; } = Array.Empty<Team>();
        public IReadOnlyList<Player> Players { get; init; } = Array.Empty<Player>();
    }
}
