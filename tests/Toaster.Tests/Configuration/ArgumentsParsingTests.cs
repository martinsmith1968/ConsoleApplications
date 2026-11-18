using System.IO;
using Ookii.CommandLine;
using Toaster.Configuration;

namespace Toaster.Tests.Configuration;

/// <summary>Exercises real command-line parsing (not just direct property assignment) to prove the
/// <see cref="AlternateSpellingEnumConverter"/> attribute wiring actually takes effect.</summary>
public class ArgumentsParsingTests
{
    [Theory]
    [InlineData("Center", TextAlignmentType.Centre)]
    [InlineData("CENTER", TextAlignmentType.Centre)]
    [InlineData("Centre", TextAlignmentType.Centre)]
    [InlineData("Left", TextAlignmentType.Left)]
    public void Parse_accepts_alternate_spelling_for_text_alignment(string value, TextAlignmentType expected)
    {
        var arguments = CommandLineParser.Parse<Arguments>(["-t", "Hello", "-ta", value]);

        arguments.ShouldNotBeNull();
        arguments.TextAlignment.ShouldBe(expected);
    }

    [Fact]
    public void Parse_accepts_alternate_spelling_for_title_alignment()
    {
        var arguments = CommandLineParser.Parse<Arguments>(["-t", "Hello", "-tia", "Center"]);

        arguments.ShouldNotBeNull();
        arguments!.TitleAlignment.ShouldBe(TextAlignmentType.Centre);
    }

    [Theory]
    [InlineData("Bold", FontStyleType.Bold)]
    [InlineData("bolditalic", FontStyleType.BoldItalic)]
    [InlineData("ITALIC", FontStyleType.Italic)]
    public void Parse_accepts_font_style_for_text_and_title(string value, FontStyleType expected)
    {
        var arguments = CommandLineParser.Parse<Arguments>(["-t", "Hello", "-tfst", value, "-tifst", value]);

        arguments.ShouldNotBeNull();
        arguments!.TextFontStyle.ShouldBe(expected);
        arguments.TitleFontStyle.ShouldBe(expected);
    }

    [Fact]
    public void Parse_returns_null_for_an_unrecognised_value()
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;
        Console.SetOut(TextWriter.Null);
        Console.SetError(TextWriter.Null);
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(["-t", "Hello", "-ta", "NotADirection"]);

            arguments.ShouldBeNull();
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
