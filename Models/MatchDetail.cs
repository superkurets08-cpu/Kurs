using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("MatchDetails")]
    public class MatchDetail
    {
        [Key]
        public int MatchDetailID { get; set; }

        public int? MatchID { get; set; }
        public int? Kills { get; set; }
        public int? Deaths { get; set; }

        [ForeignKey("MatchID")]
        public virtual Match? Match { get; set; }
    }
}