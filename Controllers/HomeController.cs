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
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;
        private readonly IReferenceDataService _referenceDataService;
        private readonly ISearchService _searchService;

        public HomeController(
            ApplicationDbContext context,
            ITournamentService tournamentService,
            ITeamService teamService,
            IPlayerService playerService,
            IReferenceDataService referenceDataService,
            ISearchService searchService)
        {
            _context = context;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _playerService = playerService;
            _referenceDataService = referenceDataService;
            _searchService = searchService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _tournamentService.GetHomePageAsync();
            return View(model);
        }

        public async Task<IActionResult> Tournaments(string? searchTerm = null, int? year = null)
        {
            var model = await _tournamentService.GetAllAsync(searchTerm, year);
            return View(model);
        }

        public async Task<IActionResult> TournamentDetails(int id)
        {
            var model = await _tournamentService.GetDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTournamentComment(TournamentCommentInputViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Комментарий должен содержать от 3 до 1000 символов.";
                return RedirectToAction(nameof(TournamentDetails), new { id = model.TournamentId });
            }

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!await _context.Tournaments.AnyAsync(t => t.TournamentID == model.TournamentId) ||
                !await _context.AppUsers.AnyAsync(u => u.AppUserId == userId && u.IsActive))
            {
                TempData["ErrorMessage"] = "Не удалось сохранить комментарий.";
                return RedirectToAction(nameof(TournamentDetails), new { id = model.TournamentId });
            }

            _context.TournamentComments.Add(new TournamentComment
            {
                TournamentId = model.TournamentId,
                AppUserId = userId,
                Content = model.Content.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Комментарий сохранён.";
            return RedirectToAction(nameof(TournamentDetails), new { id = model.TournamentId });
        }

        [Authorize(Roles = SecurityRoles.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTournamentComment(int id, int tournamentId)
        {
            var comment = await _context.TournamentComments
                .FirstOrDefaultAsync(c => c.TournamentCommentId == id && c.TournamentId == tournamentId);

            if (comment == null)
            {
                TempData["ErrorMessage"] = "Комментарий не найден.";
                return RedirectToAction(nameof(TournamentDetails), new { id = tournamentId });
            }

            _context.TournamentComments.Remove(comment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Комментарий удалён.";
            return RedirectToAction(nameof(TournamentDetails), new { id = tournamentId });
        }

        public async Task<IActionResult> Teams(string? searchTerm = null, string? country = null)
        {
            var model = await _teamService.GetAllAsync(searchTerm, country);
            return View(model);
        }

        public async Task<IActionResult> TeamDetails(int id)
        {
            var model = await _teamService.GetDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<IActionResult> CompareTeams(int? leftId = null, int? rightId = null)
        {
            var model = await _teamService.GetComparisonAsync(leftId, rightId);
            return View("CompareEntities", model);
        }

        public async Task<IActionResult> TeamHeadToHead(int? leftId = null, int? rightId = null)
        {
            var model = await _teamService.GetHeadToHeadAsync(leftId, rightId);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Players(string? searchTerm = null, string? country = null, string? role = null)
        {
            var model = await _playerService.GetPlayersPageAsync(searchTerm, country, role);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> PlayerDetails(int id)
        {
            var model = await _playerService.GetDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Heroes(string? searchTerm = null, string? attribute = null, string? attackType = null)
        {
            var model = await _referenceDataService.GetHeroesAsync(searchTerm, attribute, attackType);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> HeroDetails(int id)
        {
            var model = await _referenceDataService.GetHeroDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Items(string? searchTerm = null, string? category = null, bool neutralOnly = false)
        {
            var model = await _referenceDataService.GetItemsAsync(searchTerm, category, neutralOnly);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ItemDetails(int id)
        {
            var model = await _referenceDataService.GetItemDetailsAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> CompareItems(int? leftId = null, int? rightId = null)
        {
            var model = await _referenceDataService.GetItemComparisonAsync(leftId, rightId);
            return View("CompareEntities", model);
        }

        public async Task<IActionResult> Search(string? query = null)
        {
            var model = await _searchService.SearchAsync(query, User.Identity?.IsAuthenticated == true);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string? query)
        {
            var suggestions = await _searchService.GetSuggestionsAsync(query, User.Identity?.IsAuthenticated == true);
            return Json(suggestions);
        }
    }
}
