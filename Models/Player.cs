using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Players")]
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        public string? Nickname { get; set; }
        public string? RealName { get; set; }
        public string? Country { get; set; }
        public string? Role { get; set; }
        public string? Team { get; set; }
        public int? TeamId { get; set; }

        public string? player_url { get; set; }
        public string? AvatarUrl { get; set; }

        [ForeignKey(nameof(TeamId))]
        public virtual Team? CurrentTeam { get; set; }
        public virtual ICollection<RosterMembership> RosterMemberships { get; set; } = new List<RosterMembership>();
    }
}
