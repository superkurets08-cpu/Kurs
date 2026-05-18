using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace KURS_ASP.NET.ViewModels
{
    public class ProfileViewModel
    {
        public int AppUserId { get; init; }

        [Display(Name = "Логин")]
        public string UserName { get; init; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; init; } = string.Empty;

        [Display(Name = "Роль")]
        public string Role { get; init; } = string.Empty;

        public DateTime CreatedAtUtc { get; init; }
        public string? AvatarUrl { get; init; }
        public int CommentsCount { get; init; }
        public IReadOnlyList<string> AvailablePresetAvatars { get; init; } = Array.Empty<string>();

        [Display(Name = "Новая аватарка")]
        public IFormFile? AvatarFile { get; set; }

        [Display(Name = "Готовая аватарка")]
        public string? SelectedPresetAvatar { get; set; }
    }
}
