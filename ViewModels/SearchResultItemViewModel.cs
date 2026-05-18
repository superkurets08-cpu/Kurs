namespace KURS_ASP.NET.ViewModels
{
    public class SearchResultItemViewModel
    {
        public string Title { get; init; } = string.Empty;
        public string Subtitle { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public string Badge { get; init; } = string.Empty;
        public int Score { get; init; }
    }
}
