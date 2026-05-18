using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeamsPageViewModel> GetAllAsync(string? searchTerm = null, string? country = null)
        {
            var query = _context.Teams.AsQueryable();
            var normalizedSearch = searchTerm?.Trim();
            var normalizedCountry = country?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(t =>
                    (t.TeamName ?? string.Empty).Contains(normalizedSearch) ||
                    (t.Country ?? string.Empty).Contains(normalizedSearch));
            }

            if (!string.IsNullOrWhiteSpace(normalizedCountry))
            {
                query = query.Where(t => t.Country == normalizedCountry);
            }

            var teams = await query
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            var countries = await _context.Teams
                .Where(t => !string.IsNullOrWhiteSpace(t.Country))
                .Select(t => t.Country!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return new TeamsPageViewModel
            {
                Teams = teams,
                AvailableCountries = countries,
                SearchTerm = normalizedSearch ?? string.Empty,
                Country = normalizedCountry ?? string.Empty,
                TotalCount = teams.Count
            };
        }

        public async Task<TeamDetailsViewModel?> GetDetailsAsync(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamID == teamId);
            if (team == null)
            {
                return null;
            }

            var players = await _context.Players
                .Where(p => p.TeamId == teamId)
                .OrderBy(p => p.Nickname)
                .ToListAsync();

            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.WinnerTeam)
                .Include(m => m.Tournament)
                .Where(m => m.Team1ID == teamId || m.Team2ID == teamId)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            var rosterHistory = await _context.RosterMemberships
                .Include(r => r.Player)
                .Where(r => r.TeamId == teamId)
                .OrderByDescending(r => r.IsCurrent)
                .ThenByDescending(r => r.StartYear)
                .ThenBy(r => r.Player.Nickname)
                .ToListAsync();

            return new TeamDetailsViewModel
            {
                Team = team,
                Players = players,
                Matches = matches,
                RosterHistory = rosterHistory,
                WinsCount = matches.Count(m => m.WinnerTeamID == teamId)
            };
        }

        public async Task<ComparePageViewModel> GetComparisonAsync(int? leftId, int? rightId)
        {
            var options = await _context.Teams
                .OrderBy(t => t.TeamName)
                .Select(t => new CompareEntityOptionViewModel
                {
                    Id = t.TeamID,
                    Label = t.TeamName ?? $"Team #{t.TeamID}"
                })
                .ToListAsync();

            if (!leftId.HasValue || !rightId.HasValue)
            {
                return new ComparePageViewModel
                {
                    EntityType = "teams",
                    EntityTypeLabel = "команд",
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options,
                    Summary = "Выберите две команды, чтобы сравнить состав, архив матчей и процент побед."
                };
            }

            var teams = await _context.Teams
                .Where(t => t.TeamID == leftId || t.TeamID == rightId)
                .ToListAsync();

            if (teams.Count != 2)
            {
                return new ComparePageViewModel
                {
                    EntityType = "teams",
                    EntityTypeLabel = "команд",
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options,
                    Summary = "Не удалось загрузить обе команды для сравнения."
                };
            }

            var left = teams.First(t => t.TeamID == leftId);
            var right = teams.First(t => t.TeamID == rightId);

            var teamStats = await _context.Matches
                .Where(m => m.Team1ID == left.TeamID || m.Team2ID == left.TeamID || m.Team1ID == right.TeamID || m.Team2ID == right.TeamID)
                .ToListAsync();

            var leftMatches = teamStats.Where(m => m.Team1ID == left.TeamID || m.Team2ID == left.TeamID).ToList();
            var rightMatches = teamStats.Where(m => m.Team1ID == right.TeamID || m.Team2ID == right.TeamID).ToList();

            var leftPlayers = await _context.Players.CountAsync(p => p.TeamId == left.TeamID);
            var rightPlayers = await _context.Players.CountAsync(p => p.TeamId == right.TeamID);

            var leftWins = leftMatches.Count(m => m.WinnerTeamID == left.TeamID);
            var rightWins = rightMatches.Count(m => m.WinnerTeamID == right.TeamID);

            return new ComparePageViewModel
            {
                EntityType = "teams",
                EntityTypeLabel = "команд",
                LeftId = leftId,
                RightId = rightId,
                LeftTitle = left.TeamName ?? $"Team #{left.TeamID}",
                RightTitle = right.TeamName ?? $"Team #{right.TeamID}",
                Options = options,
                Metrics = new List<CompareMetricViewModel>
                {
                    CreateMetric("Страна", left.Country, right.Country),
                    CreateMetric("Игроков в текущем составе", leftPlayers.ToString(), rightPlayers.ToString()),
                    CreateMetric("Матчей в архиве", leftMatches.Count.ToString(), rightMatches.Count.ToString()),
                    CreateMetric("Побед", leftWins.ToString(), rightWins.ToString()),
                    CreateMetric("Winrate", ToRate(leftWins, leftMatches.Count), ToRate(rightWins, rightMatches.Count))
                },
                Summary = "Сравнение показывает, какая команда стабильнее в архиве и насколько заполнен её профиль."
            };
        }

        public async Task<TeamHeadToHeadViewModel> GetHeadToHeadAsync(int? leftId, int? rightId)
        {
            var options = await _context.Teams
                .OrderBy(t => t.TeamName)
                .Select(t => new CompareEntityOptionViewModel
                {
                    Id = t.TeamID,
                    Label = t.TeamName ?? $"Team #{t.TeamID}"
                })
                .ToListAsync();

            if (!leftId.HasValue || !rightId.HasValue)
            {
                return new TeamHeadToHeadViewModel
                {
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options
                };
            }

            var teams = await _context.Teams
                .Where(t => t.TeamID == leftId || t.TeamID == rightId)
                .ToListAsync();

            if (teams.Count != 2)
            {
                return new TeamHeadToHeadViewModel
                {
                    LeftId = leftId,
                    RightId = rightId,
                    Options = options
                };
            }

            var left = teams.First(t => t.TeamID == leftId);
            var right = teams.First(t => t.TeamID == rightId);

            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.WinnerTeam)
                .Include(m => m.Tournament)
                .Include(m => m.MVP)
                .Where(m =>
                    (m.Team1ID == left.TeamID && m.Team2ID == right.TeamID) ||
                    (m.Team1ID == right.TeamID && m.Team2ID == left.TeamID))
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            return new TeamHeadToHeadViewModel
            {
                LeftId = leftId,
                RightId = rightId,
                LeftTitle = left.TeamName ?? $"Team #{left.TeamID}",
                RightTitle = right.TeamName ?? $"Team #{right.TeamID}",
                Options = options,
                Matches = matches,
                LeftWins = matches.Count(m => m.WinnerTeamID == left.TeamID),
                RightWins = matches.Count(m => m.WinnerTeamID == right.TeamID),
                DrawLikeCount = matches.Count(m => !m.WinnerTeamID.HasValue)
            };
        }

        private static CompareMetricViewModel CreateMetric(string label, string? leftValue, string? rightValue)
        {
            return new CompareMetricViewModel
            {
                Label = label,
                LeftValue = string.IsNullOrWhiteSpace(leftValue) ? "-" : leftValue,
                RightValue = string.IsNullOrWhiteSpace(rightValue) ? "-" : rightValue
            };
        }

        private static string ToRate(int wins, int total)
        {
            if (total == 0)
            {
                return "0%";
            }

            return $"{(wins * 100.0 / total):0.#}%";
        }
    }
}
