using System.ComponentModel;
using System.Globalization;
using FigLetText.Configuration;

namespace FigLetText.Converters;

/// <summary>Accepts both "Centre" and the American "Center" spelling for <see cref="TextAlignmentType.Centre"/>.</summary>
internal sealed class AlternateSpellingTextAlignmentConverter : TypeConverter
{
    private static readonly Dictionary<string, TextAlignmentType> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(TextAlignmentType.Left)] = TextAlignmentType.Left,
        [nameof(TextAlignmentType.Right)] = TextAlignmentType.Right,
        [nameof(TextAlignmentType.Centre)] = TextAlignmentType.Centre,
        ["Center"] = TextAlignmentType.Centre,
    };

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text && Map.TryGetValue(text, out var alignment))
            return alignment;

        throw new NotSupportedException($"'{value}' is not a valid {nameof(TextAlignmentType)}. Valid values are: {string.Join(", ", Map.Keys)}");
    }
}
