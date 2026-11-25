using Spectre.Console.Cli;
using Toaster.Commands;
using Toaster.Configuration;

namespace Toaster.Tests.Commands;

/// <summary>Exercises real command-line parsing (not just direct property assignment) to prove the
/// alternate-spelling enum converter attributes on <see cref="ToastCommand.Settings"/> actually take effect.</summary>
public class ToastCommandParsingTests
{
    private sealed class ProbeCommand : Command<ToastCommand.Settings>
    {
        public static ToastCommand.Settings? Captured;

        public override int Execute(CommandContext context, ToastCommand.Settings settings, CancellationToken cancellationToken)
        {
            Captured = settings;

            return 0;
        }
    }

    private static ToastCommand.Settings Parse(params string[] args)
    {
        ProbeCommand.Captured = null;

        var app = new CommandApp<ProbeCommand>();
        app.Configure(config => config.Settings.PropagateExceptions = true);

        var result = app.Run(args);
        result.ShouldBe(0);

        return ProbeCommand.Captured!;
    }

    [Theory]
    [InlineData("Center", TextAlignmentType.Centre)]
    [InlineData("CENTER", TextAlignmentType.Centre)]
    [InlineData("Centre", TextAlignmentType.Centre)]
    [InlineData("Left", TextAlignmentType.Left)]
    public void Parse_accepts_alternate_spelling_for_text_alignment(string value, TextAlignmentType expected)
    {
        var settings = Parse("Hello", "--ta", value);

        settings.TextAlignment.ShouldBe(expected);
    }

    [Fact]
    public void Parse_accepts_alternate_spelling_for_title_alignment()
    {
        var settings = Parse("Hello", "--tia", "Center");

        settings.TitleAlignment.ShouldBe(TextAlignmentType.Centre);
    }

    [Theory]
    [InlineData("Bold", FontStyleType.Bold)]
    [InlineData("bolditalic", FontStyleType.BoldItalic)]
    [InlineData("ITALIC", FontStyleType.Italic)]
    public void Parse_accepts_font_style_for_text_and_title(string value, FontStyleType expected)
    {
        var settings = Parse("Hello", "--tfst", value, "--tifst", value);

        settings.TextFontStyle.ShouldBe(expected);
        settings.TitleFontStyle.ShouldBe(expected);
    }
}
