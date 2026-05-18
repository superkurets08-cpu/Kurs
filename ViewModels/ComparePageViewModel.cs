namespace KURS_ASP.NET.ViewModels
{
    public class ComparePageViewModel
    {
        public string EntityType { get; init; } = string.Empty;
        public string EntityTypeLabel { get; init; } = string.Empty;
        public int? LeftId { get; init; }
        public int? RightId { get; init; }
        public string LeftTitle { get; init; } = string.Empty;
        public string RightTitle { get; init; } = string.Empty;
        public IReadOnlyList<CompareEntityOptionViewModel> Options { get; init; } = Array.Empty<CompareEntityOptionViewModel>();
        public IReadOnlyList<CompareMetricViewModel> Metrics { get; init; } = Array.Empty<CompareMetricViewModel>();
        public string Summary { get; init; } = string.Empty;
        public bool HasComparison => Metrics.Count > 0;
    }
}
