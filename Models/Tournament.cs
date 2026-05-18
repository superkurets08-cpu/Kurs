using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Tournaments")]
    public class Tournament
    {
        [Key]
        public int TournamentID { get; set; }

        // ВСЕ СТРОКИ С ?
        public string? TournamentName { get; set; }
        public int Year { get; set; }
        public decimal? PrizePool { get; set; }
        public string? Location { get; set; }
        public string? ChampionTeam { get; set; }

        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
        public virtual ICollection<TournamentComment> Comments { get; set; } = new List<TournamentComment>();
    }
}
