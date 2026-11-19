using System.Windows.Media;

namespace Toaster.Services.Parsers;

internal static class FontParser
{
    /// <summary>True if empty (no font override requested) or the name of an installed font family.</summary>
    public static bool IsValid(string? text) =>
        string.IsNullOrWhiteSpace(text) ||
        Fonts.SystemFontFamilies.Any(family => string.Equals(family.Source, text.Trim(), StringComparison.OrdinalIgnoreCase));
}
