using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        public int TeamID { get; set; }

        public string? TeamName { get; set; }
        public string? Country { get; set; }
        public string? LogoURL { get; set; }
        public string? team_url { get; set; }

        public virtual ICollection<Player> Players { get; set; } = new List<Player>();
        public virtual ICollection<Match> Matches1 { get; set; } = new List<Match>();
        public virtual ICollection<Match> Matches2 { get; set; } = new List<Match>();
        public virtual ICollection<Match> WonMatches { get; set; } = new List<Match>();
        public virtual ICollection<RosterMembership> RosterMemberships { get; set; } = new List<RosterMembership>();
    }
}
