using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext _context;

        public TournamentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HomeIndexViewModel> GetHomePageAsync()
        {
            return new HomeIndexViewModel
            {
                RecentTournaments = await _context.Tournaments
                    .OrderByDescending(t => t.Year)
                    .Take(6)
                    .ToListAsync(),
                TournamentsCount = await _context.Tournaments.CountAsync(),
                TeamsCount = await _context.Teams.CountAsync(),
                PlayersCount = await _context.Players.CountAsync(),
                HeroesCount = await _context.Heroes.CountAsync()
            };
        }

        public async Task<TournamentsPageViewModel> GetAllAsync(string? searchTerm = null, int? year = null)
        {
            var query = _context.Tournaments.AsQueryable();
            var normalizedSearch = searchTerm?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(t =>
                    (t.TournamentName ?? string.Empty).Contains(normalizedSearch) ||
                    (t.Location ?? string.Empty).Contains(normalizedSearch) ||
                    (t.ChampionTeam ?? string.Empty).Contains(normalizedSearch));
            }

            if (year.HasValue)
            {
                query = query.Where(t => t.Year == year.Value);
            }

            var tournaments = await query
                .OrderByDescending(t => t.Year)
                .ToListAsync();

            var availableYears = await _context.Tournaments
                .Select(t => t.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return new TournamentsPageViewModel
            {
                Tournaments = tournaments,
                AvailableYears = availableYears,
                SearchTerm = normalizedSearch ?? string.Empty,
                Year = year,
                TotalCount = tournaments.Count
            };
        }

        public async Task<TournamentDetailsViewModel?> GetDetailsAsync(int tournamentId)
        {
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.TournamentID == tournamentId);

            if (tournament == null)
            {
                return null;
            }

            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.MVP)
                .Where(m => m.TournamentID == tournamentId)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            var comments = await _context.TournamentComments
                .Include(c => c.User)
                .Where(c => c.TournamentId == tournamentId)
                .OrderByDescending(c => c.CreatedAtUtc)
                .ToListAsync();

            return new TournamentDetailsViewModel
            {
                Tournament = tournament,
                Details = TournamentMetadata.GetDetails(tournament),
                Matches = matches,
                Comments = comments,
                CommentForm = new TournamentCommentInputViewModel
                {
                    TournamentId = tournamentId
                }
            };
        }
    }
}
