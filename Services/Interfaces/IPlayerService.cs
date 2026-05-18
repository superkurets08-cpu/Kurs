using KURS_ASP.NET.ViewModels;

namespace KURS_ASP.NET.Services.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayersPageViewModel> GetPlayersPageAsync(string? searchTerm = null, string? country = null, string? role = null);
        Task<PlayerDetailsViewModel?> GetDetailsAsync(int playerId);
    }
}
