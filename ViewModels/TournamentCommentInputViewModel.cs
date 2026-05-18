using System.ComponentModel.DataAnnotations;

namespace KURS_ASP.NET.ViewModels
{
    public class TournamentCommentInputViewModel
    {
        public int TournamentId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 3)]
        public string Content { get; set; } = string.Empty;
    }
}
