using KURS_ASP.NET.Data;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly ApplicationDbContext _context;

        public PlayerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlayersPageViewModel> GetPlayersPageAsync(string? searchTerm = null, string? country = null, string? role = null)
        {
            var query = _context.Players
                .Include(p => p.CurrentTeam)
                .Where(p => !string.IsNullOrWhiteSpace(p.Nickname))
                .AsQueryable();

            var normalizedSearch = searchTerm?.Trim();
            var normalizedCountry = country?.Trim();
            var normalizedRole = role?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(p =>
                    (p.Nickname ?? string.Empty).Contains(normalizedSearch) ||
                    (p.RealName ?? string.Empty).Contains(normalizedSearch) ||
                    (p.Country ?? string.Empty).Contains(normalizedSearch) ||
                    (p.Role ?? string.Empty).Contains(normalizedSearch) ||
                    (p.CurrentTeam != null && (p.CurrentTeam.TeamName ?? string.Empty).Contains(normalizedSearch)) ||
                    (p.Team ?? string.Empty).Contains(normalizedSearch));
            }

            if (!string.IsNullOrWhiteSpace(normalizedCountry))
            {
                query = query.Where(p => p.Country == normalizedCountry);
            }

            if (!string.IsNullOrWhiteSpace(normalizedRole))
            {
                query = query.Where(p => p.Role == normalizedRole);
            }

            var players = await query
                .OrderBy(p => p.Nickname)
                .ToListAsync();

            var allPlayers = await _context.Players.ToListAsync();

            return new PlayersPageViewModel
            {
                Players = players,
                AvailableCountries = allPlayers
                    .Select(p => p.Country)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(c => c)
                    .Cast<string>()
                    .ToList(),
                AvailableRoles = allPlayers
                    .Select(p => p.Role)
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(r => r)
                    .Cast<string>()
                    .ToList(),
                SearchTerm = normalizedSearch ?? string.Empty,
                Country = normalizedCountry ?? string.Empty,
                Role = normalizedRole ?? string.Empty,
                TotalPlayers = players.Count,
                TotalTeams = players
                    .Select(p => p.CurrentTeam?.TeamName ?? p.Team)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count(),
                TotalCountries = players
                    .Select(p => p.Country)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count()
            };
        }

        public async Task<PlayerDetailsViewModel?> GetDetailsAsync(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.CurrentTeam)
                .FirstOrDefaultAsync(p => p.PlayerID == playerId);

            if (player == null)
            {
                return null;
            }

            var team = player.CurrentTeam;

            var recentMatches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.Tournament)
                .Where(m =>
                    m.MVPPlayerID == playerId ||
                    (team != null && (m.Team1ID == team.TeamID || m.Team2ID == team.TeamID)))
                .OrderByDescending(m => m.MatchDate)
                .Take(8)
                .ToListAsync();

            var rosterHistory = await _context.RosterMemberships
                .Include(r => r.Team)
                .Where(r => r.PlayerId == playerId)
                .OrderByDescending(r => r.IsCurrent)
                .ThenByDescending(r => r.StartYear)
                .ToListAsync();

            var mvpAwardsCount = await _context.Matches.CountAsync(m => m.MVPPlayerID == playerId);

            return new PlayerDetailsViewModel
            {
                Player = player,
                Team = team,
                RecentMatches = recentMatches,
                RosterHistory = rosterHistory,
                MvpAwardsCount = mvpAwardsCount
            };
        }
    }
}
