using KURS_ASP.NET.Models;
using KURS_ASP.NET.ViewModels;

namespace KURS_ASP.NET.Services.Interfaces
{
    public interface ITeamService
    {
        Task<TeamsPageViewModel> GetAllAsync(string? searchTerm = null, string? country = null);
        Task<TeamDetailsViewModel?> GetDetailsAsync(int teamId);
        Task<ComparePageViewModel> GetComparisonAsync(int? leftId, int? rightId);
        Task<TeamHeadToHeadViewModel> GetHeadToHeadAsync(int? leftId, int? rightId);
    }
}
