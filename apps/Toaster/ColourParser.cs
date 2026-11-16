using System.Drawing;
using MediaColor = System.Windows.Media.Color;

namespace Toaster;

internal static class ColourParser
{
    /// <summary>
    /// Resolves a colour name (System.Drawing.KnownColor, case-insensitive) or a # value
    /// (#RGB, #ARGB, #RRGGBB, #AARRGGBB).
    /// </summary>
    public static bool TryParse(string? text, out MediaColor colour)
    {
        colour = default;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        text = text.Trim();

        if (Enum.TryParse<KnownColor>(text, ignoreCase: true, out var known) && Enum.IsDefined(known))
        {
            var c = Color.FromKnownColor(known);
            colour = MediaColor.FromArgb(c.A, c.R, c.G, c.B);
            return true;
        }

        if (!text.StartsWith('#'))
            return false;

        var hex = text[1..];
        if (hex.Length is 3 or 4)
            hex = string.Concat(hex.Select(ch => $"{ch}{ch}")); // expand shorthand

        if (hex.Length is not (6 or 8) || !hex.All(Uri.IsHexDigit))
            return false;

        var value = Convert.ToUInt32(hex, 16);
        colour = hex.Length == 6
            ? MediaColor.FromRgb((byte)(value >> 16), (byte)(value >> 8), (byte)value)
            : MediaColor.FromArgb((byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        return true;
    }

    /// <summary>True if dark text reads better than light text on this background.</summary>
    public static bool IsLight(MediaColor colour) =>
        (0.299 * colour.R + 0.587 * colour.G + 0.114 * colour.B) > 160;
}
