namespace BannerText.Configuration;

public enum TextAlignmentType
{
    Left = 0,
    Right,
    Center,
}

public static class TextAlignmentTypeExtensions
{
    public const string AllValues = $"{nameof(TextAlignmentType.Left)}, {nameof(TextAlignmentType.Right)}, {nameof(TextAlignmentType.Center)}";
}
