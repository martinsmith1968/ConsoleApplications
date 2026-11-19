using System.Globalization;
using Ookii.CommandLine;
using Ookii.CommandLine.Conversion;
using Toaster.Data;

namespace Toaster.Converters;

/// <summary>
/// Wraps the standard enum conversion so an enum-typed argument also accepts alternate spellings
/// (see <see cref="AlternateSpellings"/>) before falling back to normal, case-insensitive enum parsing -
/// so an unrecognised value still gets Ookii's usual "must be one of the following values" error.
/// </summary>
internal sealed class AlternateSpellingEnumConverter : ArgumentConverter
{
    public override object? Convert(ReadOnlyMemory<char> value, CultureInfo culture, CommandLineArgument argument)
    {
        var canonical = AlternateSpellings.ToCanonical(value.ToString());

        return new EnumConverter(argument.ElementType).Convert(canonical.AsMemory(), culture, argument);
    }
}
