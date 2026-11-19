using Toaster.Services.Parsers;

namespace Toaster.Tests.Services.Parsers;

public class FontParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_true_for_no_font_requested(string? text)
    {
        FontParser.IsValid(text).ShouldBeTrue();
    }

    [Theory]
    [InlineData("Arial")]
    [InlineData("Segoe UI")]
    [InlineData("arial")]
    public void IsValid_true_for_an_installed_font(string text)
    {
        FontParser.IsValid(text).ShouldBeTrue();
    }

    [Fact]
    public void IsValid_false_for_an_unknown_font()
    {
        FontParser.IsValid("NotARealFontXYZ").ShouldBeFalse();
    }
}
