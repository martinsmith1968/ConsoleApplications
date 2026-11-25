namespace BannerText.Configuration;

public sealed record BannerOptions
{
    public char HeaderLineChar { get; init; } = '*';
    public int HeaderLineCount { get; init; } = 1;
    public char FooterLineChar { get; init; } = '*';
    public int FooterLineCount { get; init; } = 1;
    public char TextLineChar { get; init; } = '*';
    public int TitlePrefixCount { get; init; } = 2;
    public int TitleSuffixCount { get; init; } = 2;
    public int TitlePrefixGapSize { get; init; } = 2;
    public int TitleSuffixGapSize { get; init; } = 2;
    public TextAlignmentType Alignment { get; init; } = TextAlignmentType.Left;
    public int MinTotalLength { get; init; }
    public int MaxTotalLength { get; init; }
}
