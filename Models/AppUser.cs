using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("AppUsers")]
    public class AppUser
    {
        [Key]
        public int AppUserId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(512)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = SecurityRoles.User;

        [StringLength(260)]
        public string? AvatarUrl { get; set; }

        [StringLength(50)]
        public string? ExternalProvider { get; set; }

        [StringLength(200)]
        public string? ExternalSubject { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<TournamentComment> TournamentComments { get; set; } = new List<TournamentComment>();
        public virtual ICollection<AdminActionLog> AdminActionLogs { get; set; } = new List<AdminActionLog>();
    }
}
