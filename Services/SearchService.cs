using KURS_ASP.NET.Data;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SearchViewModel> SearchAsync(string? query, bool includeRestrictedContent)
        {
            var normalizedQuery = query?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalizedQuery))
            {
                return new SearchViewModel();
            }

            var results = new List<SearchResultItemViewModel>();
            results.AddRange(await SearchTournamentsAsync(normalizedQuery));
            results.AddRange(await SearchTeamsAsync(normalizedQuery));

            if (includeRestrictedContent)
            {
                results.AddRange(await SearchPlayersAsync(normalizedQuery));
                results.AddRange(await SearchHeroesAsync(normalizedQuery));
                results.AddRange(await SearchItemsAsync(normalizedQuery));
            }

            var orderedResults = results
                .OrderByDescending(r => r.Score)
                .ThenBy(r => r.Category)
                .ThenBy(r => r.Title)
                .ToList();

            return new SearchViewModel
            {
                Query = normalizedQuery,
                Results = orderedResults,
                Groups = orderedResults
                    .GroupBy(r => r.Category)
                    .Select(g => new SearchCategoryGroupViewModel
                    {
                        Category = g.Key,
                        Items = g.ToList()
                    })
                    .ToList(),
                Suggestions = await GetSuggestionsAsync(normalizedQuery, includeRestrictedContent)
            };
        }

        public async Task<IReadOnlyList<SearchSuggestionViewModel>> GetSuggestionsAsync(string? query, bool includeRestrictedContent)
        {
            var normalizedQuery = query?.Trim() ?? string.Empty;
            if (normalizedQuery.Length < 2)
            {
                return Array.Empty<SearchSuggestionViewModel>();
            }

            var results = new List<SearchSuggestionViewModel>();

            results.AddRange((await _context.Tournaments
                    .Where(t => t.TournamentName != null)
                    .ToListAsync())
                .Select(t => new SearchSuggestionViewModel { Label = t.TournamentName!, Category = "Турнир", Url = $"/Home/TournamentDetails/{t.TournamentID}" }));

            results.AddRange((await _context.Teams
                    .Where(t => t.TeamName != null)
                    .ToListAsync())
                .Select(t => new SearchSuggestionViewModel { Label = t.TeamName!, Category = "Команда", Url = $"/Home/TeamDetails/{t.TeamID}" }));

            if (includeRestrictedContent)
            {
                results.AddRange((await _context.Players
                        .Where(p => p.Nickname != null)
                        .ToListAsync())
                    .Select(p => new SearchSuggestionViewModel { Label = p.Nickname!, Category = "Игрок", Url = $"/Home/PlayerDetails/{p.PlayerID}" }));

                results.AddRange((await _context.Heroes
                        .Where(h => h.HeroName != null)
                        .ToListAsync())
                    .Select(h => new SearchSuggestionViewModel { Label = h.HeroName!, Category = "Герой", Url = $"/Home/HeroDetails/{h.HeroID}" }));

                results.AddRange((await _context.Items
                        .Where(i => i.ItemName != null)
                        .ToListAsync())
                    .Select(i => new SearchSuggestionViewModel { Label = i.ItemName!, Category = "Предмет", Url = $"/Home/ItemDetails/{i.ItemID}" }));
            }

            return results
                .Select(x => new
                {
                    x.Label,
                    x.Category,
                    x.Url,
                    Score = CalculateScore(x.Label, normalizedQuery)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Label)
                .Take(6)
                .Select(x => new SearchSuggestionViewModel
                {
                    Label = x.Label,
                    Category = x.Category,
                    Url = x.Url
                })
                .ToList();
        }

        private async Task<List<SearchResultItemViewModel>> SearchTournamentsAsync(string query)
        {
            var tournaments = await _context.Tournaments.ToListAsync();
            return tournaments
                .Select(t => new SearchResultItemViewModel
                {
                    Title = t.TournamentName ?? "Tournament",
                    Subtitle = $"{t.Year} • {t.Location ?? "Location TBD"}",
                    Category = "Турниры",
                    Badge = t.ChampionTeam ?? "Archive",
                    Url = $"/Home/TournamentDetails/{t.TournamentID}",
                    Score = MaxScore(query, t.TournamentName, t.Location, t.ChampionTeam, t.Year.ToString())
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(8)
                .ToList();
        }

        private async Task<List<SearchResultItemViewModel>> SearchTeamsAsync(string query)
        {
            var teams = await _context.Teams.ToListAsync();
            return teams
                .Select(t => new SearchResultItemViewModel
                {
                    Title = t.TeamName ?? "Team",
                    Subtitle = t.Country ?? "Country not specified",
                    Category = "Команды",
                    Badge = "Team profile",
                    Url = $"/Home/TeamDetails/{t.TeamID}",
                    Score = MaxScore(query, t.TeamName, t.Country)
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(8)
                .ToList();
        }

        private async Task<List<SearchResultItemViewModel>> SearchPlayersAsync(string query)
        {
            var players = await _context.Players
                .Include(p => p.CurrentTeam)
                .ToListAsync();

            return players
                .Select(p => new SearchResultItemViewModel
                {
                    Title = p.Nickname ?? "Player",
                    Subtitle = $"{p.RealName ?? "Unknown"} • {p.CurrentTeam?.TeamName ?? p.Team ?? "No team"}",
                    Category = "Игроки",
                    Badge = p.Role ?? "Player",
                    Url = $"/Home/PlayerDetails/{p.PlayerID}",
                    Score = MaxScore(query, p.Nickname, p.RealName, p.Country, p.Role, p.CurrentTeam?.TeamName, p.Team)
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(8)
                .ToList();
        }

        private async Task<List<SearchResultItemViewModel>> SearchHeroesAsync(string query)
        {
            var heroes = await _context.Heroes.ToListAsync();
            return heroes
                .Select(h => new SearchResultItemViewModel
                {
                    Title = h.HeroName ?? "Hero",
                    Subtitle = $"{h.PrimaryAttribute ?? "Attribute"} • {h.AttackType ?? "Attack type"}",
                    Category = "Герои",
                    Badge = "Hero data",
                    Url = $"/Home/HeroDetails/{h.HeroID}",
                    Score = MaxScore(query, h.HeroName, h.PrimaryAttribute, h.AttackType)
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(8)
                .ToList();
        }

        private async Task<List<SearchResultItemViewModel>> SearchItemsAsync(string query)
        {
            var items = await _context.Items.ToListAsync();
            return items
                .Select(i => new SearchResultItemViewModel
                {
                    Title = i.ItemName ?? "Item",
                    Subtitle = $"{i.Category ?? "Category"} • {(i.IsNeutral ? "Neutral" : "Shop item")}",
                    Category = "Предметы",
                    Badge = i.Cost.HasValue ? $"{i.Cost.Value} gold" : "Item data",
                    Url = $"/Home/ItemDetails/{i.ItemID}",
                    Score = MaxScore(query, i.ItemName, i.Category, i.Subcategory, i.Description)
                })
                .Where(r => r.Score > 0)
                .OrderByDescending(r => r.Score)
                .Take(8)
                .ToList();
        }

        private static int MaxScore(string query, params string?[] fields)
        {
            return fields.Max(field => CalculateScore(field, query));
        }

        private static int CalculateScore(string? field, string query)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return 0;
            }

            var source = field.Trim().ToLowerInvariant();
            var search = query.Trim().ToLowerInvariant();

            if (source == search)
            {
                return 100;
            }

            if (source.StartsWith(search))
            {
                return 90;
            }

            if (source.Contains(search))
            {
                return 75;
            }

            if (search.Length >= 4 && LevenshteinDistance(source, search) <= 2)
            {
                return 60;
            }

            var tokens = source.Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Any(token => token.StartsWith(search) || LevenshteinDistance(token, search) <= 1))
            {
                return 50;
            }

            return 0;
        }

        private static int LevenshteinDistance(string source, string target)
        {
            var matrix = new int[source.Length + 1, target.Length + 1];

            for (var i = 0; i <= source.Length; i++)
            {
                matrix[i, 0] = i;
            }

            for (var j = 0; j <= target.Length; j++)
            {
                matrix[0, j] = j;
            }

            for (var i = 1; i <= source.Length; i++)
            {
                for (var j = 1; j <= target.Length; j++)
                {
                    var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[source.Length, target.Length];
        }
    }
}
