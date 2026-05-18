using KURS_ASP.NET.ViewModels;

namespace KURS_ASP.NET.Services.Interfaces
{
    public interface IReferenceDataService
    {
        Task<HeroesPageViewModel> GetHeroesAsync(string? searchTerm = null, string? attribute = null, string? attackType = null);
        Task<ItemsPageViewModel> GetItemsAsync(string? searchTerm = null, string? category = null, bool neutralOnly = false);
        Task<HeroDetailsViewModel?> GetHeroDetailsAsync(int heroId);
        Task<ItemDetailsViewModel?> GetItemDetailsAsync(int itemId);
        Task<ComparePageViewModel> GetItemComparisonAsync(int? leftId, int? rightId);
    }
}
