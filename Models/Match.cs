using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Matches")]
    public class Match
    {
        [Key]
        public int MatchID { get; set; }

        // ВСЕ СТРОКИ С ?
        public string? Stage { get; set; }
        public DateTime? MatchDate { get; set; }
        public int? TournamentID { get; set; }
        public int? Team1ID { get; set; }
        public int? Team2ID { get; set; }
        public int? WinnerTeamID { get; set; }
        public int? MVPPlayerID { get; set; }
        public string? match_url { get; set; }

        [ForeignKey("TournamentID")]
        public virtual Tournament? Tournament { get; set; }
        [ForeignKey("Team1ID")]
        public virtual Team? Team1 { get; set; }
        [ForeignKey("Team2ID")]
        public virtual Team? Team2 { get; set; }
        [ForeignKey("WinnerTeamID")]
        public virtual Team? WinnerTeam { get; set; }
        [ForeignKey("MVPPlayerID")]
        public virtual Player? MVP { get; set; }

        public virtual ICollection<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();
    }
}
