using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("AdminActionLogs")]
    public class AdminActionLog
    {
        [Key]
        public int AdminActionLogId { get; set; }

        public int AdminUserId { get; set; }

        [Required]
        [StringLength(60)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        [StringLength(60)]
        public string EntityType { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        [StringLength(400)]
        public string? Description { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AdminUserId))]
        public virtual AppUser AdminUser { get; set; } = null!;
    }
}
