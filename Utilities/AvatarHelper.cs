namespace KURS_ASP.NET.Utilities
{
    public static class AvatarHelper
    {
        public static string GetInitials(string? value, int maxLetters = 2)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "?";
            }

            var parts = value
                .Split(new[] { ' ', '-', '_', '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

            if (parts.Length == 0)
            {
                return "?";
            }

            if (parts.Length == 1)
            {
                return parts[0].Length >= maxLetters
                    ? parts[0][..Math.Min(maxLetters, parts[0].Length)].ToUpperInvariant()
                    : parts[0].ToUpperInvariant();
            }

            return string.Concat(parts.Select(part => char.ToUpperInvariant(part[0])).Take(maxLetters));
        }
    }
}
