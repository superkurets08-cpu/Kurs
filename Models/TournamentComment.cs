using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("TournamentComments")]
    public class TournamentComment
    {
        [Key]
        public int TournamentCommentId { get; set; }

        public int TournamentId { get; set; }
        public int AppUserId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TournamentId))]
        public virtual Tournament Tournament { get; set; } = null!;

        [ForeignKey(nameof(AppUserId))]
        public virtual AppUser User { get; set; } = null!;
    }
}
