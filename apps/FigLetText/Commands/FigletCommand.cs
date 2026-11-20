using System.ComponentModel;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using FigLetText.Configuration;
using FigLetText.Converters;
using FigLetText.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FigLetText.Commands;

[Description("Render text as large FIGlet-style ASCII art in the console")]
public class FigletCommand : Command<FigletCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => CommonCommandApp.DeriveCommandName<FigletCommand>();

    public sealed class Settings : CommandSettings
    {
        [Description("The text to render")]
        [CommandArgument(0, "<text>")]
        public string Text { get; set; } = string.Empty;

        [Description("Name of a built-in FIGlet font to use. Mutually exclusive with FontFile")]
        [CommandOption($"--fn|--{nameof(FontName)}", isRequired: false)]
        public string? FontName { get; set; }

        [Description("Path to an external FIGlet (.flf) font file to use. Mutually exclusive with FontName")]
        [CommandOption($"--ff|--{nameof(FontFile)}", isRequired: false)]
        public string? FontFile { get; set; }

        [Description("The alignment of the rendered text within the console")]
        [CommandOption($"--ta|--{nameof(Alignment)}", isRequired: false)]
        [DefaultValue(TextAlignmentType.Left)]
        [TypeConverter(typeof(AlternateSpellingTextAlignmentConverter))]
        public TextAlignmentType Alignment { get; set; }

        public void CustomValidate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new ArgumentException("Value is required", nameof(Text));

            if (!string.IsNullOrWhiteSpace(FontName) && !string.IsNullOrWhiteSpace(FontFile))
                throw new ArgumentException($"Specify only one of {nameof(FontName)} or {nameof(FontFile)}, not both");

            if (!FigletFontResolver.IsValidFontName(FontName))
                throw new ArgumentException($"'{FontName}' is not a known built-in font name", nameof(FontName));

            if (!FigletFontResolver.IsValidFontFile(FontFile))
                throw new ArgumentException($"'{FontFile}' does not exist or is not a readable file", nameof(FontFile));
        }
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>
        {
            { [ "\"Hello World\"" ] },
            { [ "\"Hello World\"", "--fn", "Slant" ] },
            { [ "\"Hello World\"", "--ta", "Centre" ] },
            { [ "\"Hello World\"", "--ff", "C:\\Fonts\\custom.flf" ] },
        };
    }

    public static string[] GetNotes()
    {
        var notes = new List<string>();

        var fonts = FigletFontResolver.BuiltInFontsByName.Value
            .Select(f => f.Key)
            .ToArray();

        if (fonts.Any())
        {
            notes.Add("Built-In Fonts: " + string.Join(", ", fonts));
        }

        return
        [
            .. notes
        ];
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        try
        {
            settings.CustomValidate();

            return base.Validate(context, settings);
        }
        catch (Exception ex)
        {
            return ValidationResult.Error(ex.Message);
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var font = FigletFontResolver.Resolve(settings.FontName, settings.FontFile);
        var rendered = font.Render(settings.Text);

        FigletRenderer.WriteAligned(rendered, settings.Alignment);

        return 0;
    }
}
