using System.ComponentModel;
using System.Globalization;
using Toaster.Data;

namespace Toaster.Converters;

/// <summary>
/// Wraps the standard enum conversion so an enum-typed option also accepts alternate spellings
/// (see <see cref="AlternateSpellings"/>) before falling back to normal, case-insensitive enum parsing -
/// so an unrecognised value still gets the usual "must be one of the following values" error.
/// </summary>
public sealed class AlternateSpellingEnumConverter<TEnum> : TypeConverter
    where TEnum : struct, Enum
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string text)
        {
            var canonical = AlternateSpellings.ToCanonical(text);

            return Enum.Parse<TEnum>(canonical, ignoreCase: true);
        }

        return base.ConvertFrom(context, culture, value)!;
    }
}
