using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("RosterMemberships")]
    public class RosterMembership
    {
        [Key]
        public int RosterMembershipId { get; set; }

        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public bool IsCurrent { get; set; }

        [StringLength(120)]
        public string? Label { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public virtual Player Player { get; set; } = null!;

        [ForeignKey(nameof(TeamId))]
        public virtual Team Team { get; set; } = null!;
    }
}
