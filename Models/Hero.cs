using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Heroes")]
    public class Hero
    {
        [Key]
        public int HeroID { get; set; }

        public string? HeroName { get; set; }
        public string? PrimaryAttribute { get; set; }
        public string? AttackType { get; set; }
        public string? AvatarUrl { get; set; }

        // Прирост атрибутов за уровень
        public decimal? StrengthGain { get; set; }
        public decimal? AgilityGain { get; set; }
        public decimal? IntelligenceGain { get; set; }
    }
}
