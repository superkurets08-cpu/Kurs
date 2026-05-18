using KURS_ASP.NET.Models;
using KURS_ASP.NET.ViewModels;

namespace KURS_ASP.NET.Services.Interfaces
{
    public interface ITournamentService
    {
        Task<HomeIndexViewModel> GetHomePageAsync();
        Task<TournamentsPageViewModel> GetAllAsync(string? searchTerm = null, int? year = null);
        Task<TournamentDetailsViewModel?> GetDetailsAsync(int tournamentId);
    }
}
