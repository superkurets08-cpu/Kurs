using System.Security.Claims;
using KURS_ASP.NET.Data;
using KURS_ASP.NET.Models;
using KURS_ASP.NET.Services.Interfaces;
using KURS_ASP.NET.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KURS_ASP.NET.Controllers
{
    [Authorize(Roles = SecurityRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IWebHostEnvironment _environment;

        public AdminController(
            ApplicationDbContext context,
            IPasswordHasherService passwordHasher,
            IWebHostEnvironment environment)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                UsersCount = await _context.AppUsers.CountAsync(),
                TournamentsCount = await _context.Tournaments.CountAsync(),
                TeamsCount = await _context.Teams.CountAsync(),
                PlayersCount = await _context.Players.CountAsync(),
                MatchesCount = await _context.Matches.CountAsync(),
                HeroesCount = await _context.Heroes.CountAsync(),
                ItemsCount = await _context.Items.CountAsync(),
                AdminLogsCount = await _context.AdminActionLogs.CountAsync(),
                RecentAdminActions = await _context.AdminActionLogs
                    .Include(l => l.AdminUser)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .Take(12)
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> Tournaments()
        {
            var tournaments = await _context.Tournaments
                .OrderByDescending(t => t.Year)
                .ToListAsync();

            return View(tournaments);
        }

        public IActionResult CreateTournament() => View("TournamentForm", new Tournament());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTournament(Tournament tournament)
        {
            if (!ModelState.IsValid)
            {
                return View("TournamentForm", tournament);
            }

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "Tournament", tournament.TournamentID, $"Создан турнир {tournament.TournamentName}");
            TempData["SuccessMessage"] = "Турнир добавлен.";
            return RedirectToAction(nameof(Tournaments));
        }

        public async Task<IActionResult> EditTournament(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            return View("TournamentForm", tournament);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTournament(Tournament tournament)
        {
            if (!ModelState.IsValid)
            {
                return View("TournamentForm", tournament);
            }

            _context.Tournaments.Update(tournament);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "Tournament", tournament.TournamentID, $"Обновлён турнир {tournament.TournamentName}");
            TempData["SuccessMessage"] = "Турнир обновлён.";
            return RedirectToAction(nameof(Tournaments));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            return await DeleteEntityAsync(
                await _context.Tournaments.FindAsync(id),
                nameof(Tournaments),
                "Турнир удалён.",
                "Не удалось удалить турнир. Возможно, он связан с матчами.");
        }

        public async Task<IActionResult> Teams()
        {
            var teams = await _context.Teams
                .OrderBy(t => t.TeamName)
                .ToListAsync();

            return View(teams);
        }

        public IActionResult CreateTeam() => View("TeamForm", new Team());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeam(Team team)
        {
            if (!ModelState.IsValid)
            {
                return View("TeamForm", team);
            }

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "Team", team.TeamID, $"Создана команда {team.TeamName}");
            TempData["SuccessMessage"] = "Команда добавлена.";
            return RedirectToAction(nameof(Teams));
        }

        public async Task<IActionResult> EditTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return View("TeamForm", team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeam(Team team)
        {
            if (!ModelState.IsValid)
            {
                return View("TeamForm", team);
            }

            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "Team", team.TeamID, $"Обновлена команда {team.TeamName}");
            TempData["SuccessMessage"] = "Команда обновлена.";
            return RedirectToAction(nameof(Teams));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            return await DeleteEntityAsync(
                await _context.Teams.FindAsync(id),
                nameof(Teams),
                "Команда удалена.",
                "Не удалось удалить команду. Возможно, она участвует в матчах.");
        }

        public async Task<IActionResult> Players()
        {
            var players = await _context.Players
                .Include(p => p.CurrentTeam)
                .OrderBy(p => p.Nickname)
                .ToListAsync();

            return View(players);
        }

        public async Task<IActionResult> CreatePlayer()
        {
            await PopulateTeamsAsync();
            return View("PlayerForm", new Player());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayer(Player player, IFormFile? avatarFile)
        {
            NormalizePlayerTeam(player);

            if (!ModelState.IsValid)
            {
                await PopulateTeamsAsync();
                return View("PlayerForm", player);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                player.AvatarUrl = await SaveAvatarAsync(avatarFile, "players", $"player-{Slugify(player.Nickname, "player")}");
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            await SyncRosterMembershipAsync(player);
            await LogAdminActionAsync("Create", "Player", player.PlayerID, $"Создан игрок {player.Nickname}");
            TempData["SuccessMessage"] = "Игрок добавлен.";
            return RedirectToAction(nameof(Players));
        }

        public async Task<IActionResult> EditPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            await PopulateTeamsAsync();
            return View("PlayerForm", player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlayer(Player player, IFormFile? avatarFile)
        {
            NormalizePlayerTeam(player);

            if (!ModelState.IsValid)
            {
                await PopulateTeamsAsync();
                return View("PlayerForm", player);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                player.AvatarUrl = await SaveAvatarAsync(avatarFile, "players", $"player-{Slugify(player.Nickname, "player")}");
            }

            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            await SyncRosterMembershipAsync(player);
            await LogAdminActionAsync("Edit", "Player", player.PlayerID, $"Обновлён игрок {player.Nickname}");
            TempData["SuccessMessage"] = "Игрок обновлён.";
            return RedirectToAction(nameof(Players));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            return await DeleteEntityAsync(
                await _context.Players.FindAsync(id),
                nameof(Players),
                "Игрок удалён.",
                "Не удалось удалить игрока. Возможно, он используется в матчах.");
        }

        public async Task<IActionResult> Matches()
        {
            var matches = await _context.Matches
                .Include(m => m.Tournament)
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.WinnerTeam)
                .Include(m => m.MVP)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            return View(matches);
        }

        public async Task<IActionResult> CreateMatch()
        {
            return View("MatchForm", await BuildMatchFormModelAsync(new Match()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMatch(Match match)
        {
            if (!ModelState.IsValid)
            {
                return View("MatchForm", await BuildMatchFormModelAsync(match));
            }

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "Match", match.MatchID, $"Создан матч {match.Stage}");
            TempData["SuccessMessage"] = "Матч добавлен.";
            return RedirectToAction(nameof(Matches));
        }

        public async Task<IActionResult> EditMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            return View("MatchForm", await BuildMatchFormModelAsync(match));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMatch(Match match)
        {
            if (!ModelState.IsValid)
            {
                return View("MatchForm", await BuildMatchFormModelAsync(match));
            }

            _context.Matches.Update(match);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "Match", match.MatchID, $"Обновлён матч {match.Stage}");
            TempData["SuccessMessage"] = "Матч обновлён.";
            return RedirectToAction(nameof(Matches));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            return await DeleteEntityAsync(
                await _context.Matches.FindAsync(id),
                nameof(Matches),
                "Матч удалён.",
                "Не удалось удалить матч.");
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.AppUsers
                .OrderByDescending(u => u.CreatedAtUtc)
                .ToListAsync();

            return View(users);
        }

        public IActionResult CreateUser()
        {
            return View("UserForm", new AdminUserFormViewModel
            {
                Role = SecurityRoles.User,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(AdminUserFormViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    ModelState.AddModelError(nameof(model.NewPassword), "Укажите пароль для нового пользователя.");
                }

                return View("UserForm", model);
            }

            var user = new AppUser
            {
                UserName = model.UserName.Trim(),
                Email = model.Email.Trim(),
                Role = model.Role,
                IsActive = model.IsActive,
                PasswordHash = _passwordHasher.HashPassword(model.NewPassword),
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "User", user.AppUserId, $"Создан пользователь {user.UserName}");
            TempData["SuccessMessage"] = "Пользователь создан.";
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View("UserForm", new AdminUserFormViewModel
            {
                AppUserId = user.AppUserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(AdminUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserForm", model);
            }

            var user = await _context.AppUsers.FindAsync(model.AppUserId);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName.Trim();
            user.Email = model.Email.Trim();
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(model.NewPassword);
            }

            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "User", user.AppUserId, $"Обновлён пользователь {user.UserName}");
            TempData["SuccessMessage"] = "Пользователь обновлён.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return await DeleteEntityAsync(
                await _context.AppUsers.FindAsync(id),
                nameof(Users),
                "Пользователь удалён.",
                "Не удалось удалить пользователя.");
        }

        public async Task<IActionResult> Heroes()
        {
            var heroes = await _context.Heroes
                .OrderBy(h => h.HeroName)
                .ToListAsync();

            return View(heroes);
        }

        public IActionResult CreateHero() => View("HeroForm", new Hero());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHero(Hero hero, IFormFile? avatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View("HeroForm", hero);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                hero.AvatarUrl = await SaveAvatarAsync(avatarFile, "heroes", $"hero-{Slugify(hero.HeroName, "hero")}");
            }

            _context.Heroes.Add(hero);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "Hero", hero.HeroID, $"Создан герой {hero.HeroName}");
            TempData["SuccessMessage"] = "Герой добавлен.";
            return RedirectToAction(nameof(Heroes));
        }

        public async Task<IActionResult> EditHero(int id)
        {
            var hero = await _context.Heroes.FindAsync(id);
            if (hero == null)
            {
                return NotFound();
            }

            return View("HeroForm", hero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHero(Hero hero, IFormFile? avatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View("HeroForm", hero);
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                hero.AvatarUrl = await SaveAvatarAsync(avatarFile, "heroes", $"hero-{Slugify(hero.HeroName, "hero")}");
            }

            _context.Heroes.Update(hero);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "Hero", hero.HeroID, $"Обновлён герой {hero.HeroName}");
            TempData["SuccessMessage"] = "Герой обновлён.";
            return RedirectToAction(nameof(Heroes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHero(int id)
        {
            return await DeleteEntityAsync(
                await _context.Heroes.FindAsync(id),
                nameof(Heroes),
                "Герой удалён.",
                "Не удалось удалить героя.");
        }

        public async Task<IActionResult> Items()
        {
            var items = await _context.Items
                .OrderBy(i => i.ItemName)
                .ToListAsync();

            return View(items);
        }

        public IActionResult CreateItem() => View("ItemForm", new Item());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View("ItemForm", item);
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Create", "Item", item.ItemID, $"Создан предмет {item.ItemName}");
            TempData["SuccessMessage"] = "Предмет добавлен.";
            return RedirectToAction(nameof(Items));
        }

        public async Task<IActionResult> EditItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View("ItemForm", item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View("ItemForm", item);
            }

            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            await LogAdminActionAsync("Edit", "Item", item.ItemID, $"Обновлён предмет {item.ItemName}");
            TempData["SuccessMessage"] = "Предмет обновлён.";
            return RedirectToAction(nameof(Items));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int id)
        {
            return await DeleteEntityAsync(
                await _context.Items.FindAsync(id),
                nameof(Items),
                "Предмет удалён.",
                "Не удалось удалить предмет.");
        }

        private async Task<IActionResult> DeleteEntityAsync<TEntity>(
            TEntity? entity,
            string redirectAction,
            string successMessage,
            string errorMessage)
            where TEntity : class
        {
            if (entity == null)
            {
                TempData["ErrorMessage"] = "Запись не найдена.";
                return RedirectToAction(redirectAction);
            }

            try
            {
                var entityType = typeof(TEntity).Name;
                var entityId = TryExtractEntityId(entity);

                _context.Remove(entity);
                await _context.SaveChangesAsync();
                await LogAdminActionAsync("Delete", entityType, entityId, successMessage);
                TempData["SuccessMessage"] = successMessage;
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction(redirectAction);
        }

        private async Task PopulateTeamsAsync()
        {
            ViewBag.Teams = await _context.Teams
                .OrderBy(t => t.TeamName)
                .ToListAsync();
        }

        private void NormalizePlayerTeam(Player player)
        {
            if (player.TeamId.HasValue)
            {
                player.Team = _context.Teams
                    .Where(t => t.TeamID == player.TeamId.Value)
                    .Select(t => t.TeamName)
                    .FirstOrDefault();
            }
            else
            {
                player.Team = null;
            }
        }

        private async Task SyncRosterMembershipAsync(Player player)
        {
            var memberships = await _context.RosterMemberships
                .Where(r => r.PlayerId == player.PlayerID)
                .ToListAsync();

            foreach (var membership in memberships)
            {
                membership.IsCurrent = membership.TeamId == player.TeamId;
                if (!membership.IsCurrent && membership.EndYear == null)
                {
                    membership.EndYear = DateTime.UtcNow.Year;
                }
            }

            if (player.TeamId.HasValue && memberships.All(r => r.TeamId != player.TeamId.Value))
            {
                _context.RosterMemberships.Add(new RosterMembership
                {
                    PlayerId = player.PlayerID,
                    TeamId = player.TeamId.Value,
                    StartYear = DateTime.UtcNow.Year,
                    EndYear = null,
                    IsCurrent = true,
                    Label = "Назначен через админ-панель"
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<AdminMatchFormViewModel> BuildMatchFormModelAsync(Match match)
        {
            return new AdminMatchFormViewModel
            {
                Match = match,
                Tournaments = await _context.Tournaments.OrderByDescending(t => t.Year).ToListAsync(),
                Teams = await _context.Teams.OrderBy(t => t.TeamName).ToListAsync(),
                Players = await _context.Players.OrderBy(p => p.Nickname).ToListAsync()
            };
        }

        private async Task LogAdminActionAsync(string actionType, string entityType, int? entityId, string description)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var adminUserId))
            {
                return;
            }

            _context.AdminActionLogs.Add(new AdminActionLog
            {
                AdminUserId = adminUserId,
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                CreatedAtUtc = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        private async Task<string> SaveAvatarAsync(IFormFile file, string category, string prefix)
        {
            var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "avatars", category);
            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(file.FileName);
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".png", ".jpg", ".jpeg", ".webp", ".gif"
            };
            var safeExtension = allowedExtensions.Contains(extension) ? extension : ".png";
            var fileName = $"{prefix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}{safeExtension}";
            var fullPath = Path.Combine(uploadDirectory, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await file.CopyToAsync(stream);

            return $"/images/avatars/{category}/{fileName}";
        }

        private static string Slugify(string? value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            var normalized = new string(value
                .Trim()
                .ToLowerInvariant()
                .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
                .ToArray());

            var parts = normalized.Split('-', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? fallback : string.Join("-", parts);
        }

        private static int? TryExtractEntityId<TEntity>(TEntity entity)
            where TEntity : class
        {
            var property = entity.GetType().GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase) || p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase));

            if (property?.GetValue(entity) is int id)
            {
                return id;
            }

            var value = property?.GetValue(entity);
            if (value is null)
            {
                return null;
            }

            if (int.TryParse(value.ToString(), out var parsedId))
            {
                return parsedId;
            }

            return null;
        }
    }
}
