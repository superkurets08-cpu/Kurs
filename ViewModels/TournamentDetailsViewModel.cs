using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class TournamentDetailsViewModel
    {
        public Tournament Tournament { get; init; } = new();
        public TournamentDetailsDto Details { get; init; } = new();
        public IReadOnlyList<Match> Matches { get; init; } = Array.Empty<Match>();
        public IReadOnlyList<TournamentComment> Comments { get; init; } = Array.Empty<TournamentComment>();
        public TournamentCommentInputViewModel CommentForm { get; init; } = new();
    }
}
