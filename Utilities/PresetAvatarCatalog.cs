namespace KURS_ASP.NET.Utilities
{
    public static class PresetAvatarCatalog
    {
        public static readonly IReadOnlyList<string> All = new[]
        {
            "/images/avatars/presets/aegis.svg",
            "/images/avatars/presets/ember.svg",
            "/images/avatars/presets/frost.svg",
            "/images/avatars/presets/storm.svg",
            "/images/avatars/presets/void.svg",
            "/images/avatars/presets/forest.svg"
        };

        public static bool Contains(string? avatarUrl)
        {
            return !string.IsNullOrWhiteSpace(avatarUrl) &&
                   All.Contains(avatarUrl, StringComparer.OrdinalIgnoreCase);
        }
    }
}
