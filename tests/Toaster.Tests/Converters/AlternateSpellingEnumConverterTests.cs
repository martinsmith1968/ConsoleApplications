using Toaster.Configuration;
using Toaster.Converters;

namespace Toaster.Tests.Converters;

public class AlternateSpellingEnumConverterTests
{
    [Theory]
    [InlineData("Center", TextAlignmentType.Centre)]
    [InlineData("CENTER", TextAlignmentType.Centre)]
    [InlineData("Centre", TextAlignmentType.Centre)]
    [InlineData("Left", TextAlignmentType.Left)]
    public void ConvertFrom_accepts_alternate_spelling(string value, TextAlignmentType expected)
    {
        var converter = new AlternateSpellingEnumConverter<TextAlignmentType>();

        var result = converter.ConvertFrom(null, null, value);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("Bold", FontStyleType.Bold)]
    [InlineData("bolditalic", FontStyleType.BoldItalic)]
    [InlineData("ITALIC", FontStyleType.Italic)]
    public void ConvertFrom_is_case_insensitive_for_other_enum_types(string value, FontStyleType expected)
    {
        var converter = new AlternateSpellingEnumConverter<FontStyleType>();

        var result = converter.ConvertFrom(null, null, value);

        result.ShouldBe(expected);
    }

    [Fact]
    public void ConvertFrom_throws_for_an_unrecognised_value()
    {
        var converter = new AlternateSpellingEnumConverter<TextAlignmentType>();

        Should.Throw<ArgumentException>(() => converter.ConvertFrom(null, null, "NotADirection"));
    }

    [Fact]
    public void CanConvertFrom_accepts_string()
    {
        var converter = new AlternateSpellingEnumConverter<TextAlignmentType>();

        converter.CanConvertFrom(null, typeof(string)).ShouldBeTrue();
    }
}
