using BannerText.Configuration;

namespace BannerText.Services;

/// <remarks>
/// Ported from the canonical implementations at
/// https://github.com/martinsmith1968/NativeConsoleApplicationsRUST/blob/main/bannertext/src/lib.rs
/// and https://github.com/martinsmith1968/NativeWindowsConsoleApplicationsCPP/tree/master/apps/BannerText
/// so output stays consistent across all three ports.
/// </remarks>
internal static class BannerRenderer
{
    public static string[] Generate(IReadOnlyList<string> texts, BannerOptions options)
    {
        var prefixChars = new string(options.TextLineChar, options.TitlePrefixCount);
        var prefixGap = new string(' ', options.TitlePrefixGapSize);
        var suffixGap = new string(' ', options.TitleSuffixGapSize);
        var suffixChars = new string(options.TextLineChar, options.TitleSuffixCount);

        var prefixTotal = options.TitlePrefixCount + options.TitlePrefixGapSize;
        var suffixTotal = options.TitleSuffixCount + options.TitleSuffixGapSize;

        var maxTextLength = texts.Count > 0 ? texts.Max(t => t.Length) : 0;
        var naturalLength = prefixTotal + maxTextLength + suffixTotal;

        var totalLength = naturalLength;
        if (options.MinTotalLength > 0)
            totalLength = Math.Max(totalLength, options.MinTotalLength);
        if (options.MaxTotalLength > 0)
            totalLength = Math.Min(totalLength, options.MaxTotalLength);

        var textAreaWidth = Math.Max(0, totalLength - (prefixTotal + suffixTotal));

        var headerLine = new string(options.HeaderLineChar, totalLength);
        var footerLine = new string(options.FooterLineChar, totalLength);

        var lines = new List<string>();

        for (var i = 0; i < options.HeaderLineCount; i++)
            lines.Add(headerLine);

        foreach (var text in texts)
        {
            var displayText = text.Length > textAreaWidth ? text[..textAreaWidth] : text;
            var alignedText = Align(displayText, options.Alignment, textAreaWidth);

            lines.Add($"{prefixChars}{prefixGap}{alignedText}{suffixGap}{suffixChars}");
        }

        for (var i = 0; i < options.FooterLineCount; i++)
            lines.Add(footerLine);

        return lines.ToArray();
    }

    private static string Align(string text, TextAlignmentType alignment, int width)
    {
        return alignment switch
        {
            TextAlignmentType.Right => text.PadLeft(width),
            TextAlignmentType.Center => PadCenter(text, width),
            _ => text.PadRight(width),
        };
    }

    private static string PadCenter(string text, int width)
    {
        var padding = width - text.Length;
        var left = padding / 2;
        var right = padding - left;

        return new string(' ', left) + text + new string(' ', right);
    }
}
