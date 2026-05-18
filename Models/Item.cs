using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KURS_ASP.NET.Models
{
    [Table("Items")]
    public class Item
    {
        [Key]
        public int ItemID { get; set; }

        public string? ItemName { get; set; }

        // ИСПРАВЛЕНО: int? вместо decimal?
        public int? Cost { get; set; }

        public string? Category { get; set; }
        public string? Subcategory { get; set; }
        public string? Description { get; set; }
        public bool IsNeutral { get; set; }
        public int? NeutralTier { get; set; }
        public bool IsRemoved { get; set; }
    }
}