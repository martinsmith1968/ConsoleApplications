namespace Toaster.Data;

/// <summary>
/// Maps alternate spellings of fixed argument values to their canonical name, e.g. the American
/// "Center" for the British "Centre" used by <see cref="Configuration.TextAlignmentType.Centre"/>.
/// Add more entries here as other spelling variants come up; lookups are case-insensitive.
/// </summary>
internal static class AlternateSpellings
{
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Center"] = "Centre",
    };

    /// <summary>The canonical name for <paramref name="text"/>, or <paramref name="text"/> unchanged if it isn't a known alternate spelling.</summary>
    public static string ToCanonical(string text) =>
        Map.TryGetValue(text, out var canonical) ? canonical : text;
}
