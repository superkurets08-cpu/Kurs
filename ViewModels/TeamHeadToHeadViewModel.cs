using KURS_ASP.NET.Models;

namespace KURS_ASP.NET.ViewModels
{
    public class TeamHeadToHeadViewModel
    {
        public int? LeftId { get; init; }
        public int? RightId { get; init; }
        public string LeftTitle { get; init; } = string.Empty;
        public string RightTitle { get; init; } = string.Empty;
        public IReadOnlyList<CompareEntityOptionViewModel> Options { get; init; } = Array.Empty<CompareEntityOptionViewModel>();
        public IReadOnlyList<Match> Matches { get; init; } = Array.Empty<Match>();
        public int LeftWins { get; init; }
        public int RightWins { get; init; }
        public int DrawLikeCount { get; init; }
        public bool HasSelection => LeftId.HasValue && RightId.HasValue;
    }
}
