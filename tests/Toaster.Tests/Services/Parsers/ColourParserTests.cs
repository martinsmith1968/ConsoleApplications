using System.Windows.Media;
using Toaster.Services.Parsers;

namespace Toaster.Tests.Services.Parsers;

public class ColourParserTests
{
    [Theory]
    [InlineData("Red")]
    [InlineData("red")]
    [InlineData("CornflowerBlue")]
    [InlineData("  Red  ")]
    public void TryParse_known_colour_name_succeeds(string name)
    {
        ColourParser.TryParse(name, out _).ShouldBeTrue();
    }

    [Theory]
    [InlineData("#F00", (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x00)] // RGB shorthand
    [InlineData("#8F00", (byte)0x88, (byte)0xFF, (byte)0x00, (byte)0x00)] // ARGB shorthand
    [InlineData("#FF0000", (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x00)] // RRGGBB
    [InlineData("#ff0000", (byte)0xFF, (byte)0xFF, (byte)0x00, (byte)0x00)] // lower-case RRGGBB
    [InlineData("#80FF0000", (byte)0x80, (byte)0xFF, (byte)0x00, (byte)0x00)] // AARRGGBB
    public void TryParse_hex_value_succeeds(string text, byte a, byte r, byte g, byte b)
    {
        ColourParser.TryParse(text, out var colour).ShouldBeTrue();

        colour.A.ShouldBe(a);
        colour.R.ShouldBe(r);
        colour.G.ShouldBe(g);
        colour.B.ShouldBe(b);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("NotAColour")]
    [InlineData("#12")]
    [InlineData("#12345")]
    [InlineData("#GGGGGG")]
    [InlineData("NotAColour#FF0000")]
    public void TryParse_garbage_fails(string? text)
    {
        ColourParser.TryParse(text, out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("Control")]
    [InlineData("Window")]
    [InlineData("Desktop")]
    [InlineData("Highlight")]
    [InlineData("control")]
    public void TryParse_system_colour_is_rejected(string name)
    {
        ColourParser.TryParse(name, out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData( "auto", true)]
    [InlineData("AUTO", true)]
    [InlineData("Red", false)]
    public void IsAuto_detects_auto_or_empty(string? text, bool expected)
    {
        ColourParser.IsAuto(text).ShouldBe(expected);
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("#FF0000")]
    public void TryParsePair_single_colour_succeeds_with_no_second_colour(string text)
    {
        ColourParser.TryParsePair(text, out var first, out var second).ShouldBeTrue();

        first.ShouldBe(Colors.Red);
        second.ShouldBeNull();
    }

    [Fact]
    public void TryParsePair_two_colours_succeeds()
    {
        ColourParser.TryParsePair("Red:Blue", out var first, out var second).ShouldBeTrue();

        first.ShouldBe(Colors.Red);
        second.ShouldBe(Colors.Blue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Red:Green:Blue")]
    [InlineData("NotAColour:Blue")]
    [InlineData("Red:NotAColour")]
    public void TryParsePair_garbage_fails(string? text)
    {
        ColourParser.TryParsePair(text, out _, out _).ShouldBeFalse();
    }

    [Fact]
    public void IsLight_true_for_white()
    {
        ColourParser.IsLight(Colors.White).ShouldBeTrue();
    }

    [Fact]
    public void IsLight_false_for_black()
    {
        ColourParser.IsLight(Colors.Black).ShouldBeFalse();
    }
}
