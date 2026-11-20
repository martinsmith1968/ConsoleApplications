using System.Reflection;
using Figgle;
using Figgle.Fonts;

namespace FigLetText.Services;

internal static class FigletFontResolver
{
    internal static readonly Lazy<IReadOnlyDictionary<string, PropertyInfo>> BuiltInFontsByName = new(() =>
        typeof(FiggleFonts)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p => p.PropertyType == typeof(FiggleFont))
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase));

    /// <summary>True if empty (no font name requested) or the case-insensitive name of a built-in font.</summary>
    public static bool IsValidFontName(string? name) =>
        string.IsNullOrWhiteSpace(name) || BuiltInFontsByName.Value.ContainsKey(name.Trim());

    /// <summary>True if empty (no font file requested) or an existing, readable file.</summary>
    public static bool IsValidFontFile(string? path) =>
        string.IsNullOrWhiteSpace(path) || File.Exists(path);

    /// <summary>Resolves the <see cref="FiggleFont"/> to use, preferring an external font file over a built-in font name, defaulting to <see cref="FiggleFonts.Standard"/>.</summary>
    public static FiggleFont Resolve(string? fontName, string? fontFile)
    {
        if (!string.IsNullOrWhiteSpace(fontFile))
        {
            using var stream = File.OpenRead(fontFile);

            return FiggleFontParser.Parse(stream);
        }

        if (!string.IsNullOrWhiteSpace(fontName) && BuiltInFontsByName.Value.TryGetValue(fontName.Trim(), out var property))
            return (FiggleFont)property.GetValue(null)!;

        return FiggleFonts.Standard;
    }
}
