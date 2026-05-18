using System.ComponentModel.DataAnnotations;

namespace KURS_ASP.NET.ViewModels
{
    public class AdminUserFormViewModel
    {
        public int AppUserId { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
