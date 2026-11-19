using System.ComponentModel;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Toaster.Configuration;
using Toaster.Requests;
using Toaster.Services;
using Toaster.Services.Parsers;

namespace Toaster.Commands;

[Description("Show a Toast popup")]
public class ToastCommand : Command<ToastCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => CommonCommandApp.DeriveCommandName<ToastCommand>();

    public sealed class Settings : CommandSettings
    {
        [Description("The text to display")]
        [CommandArgument(0, "<text>")]
        public string Text { get; set; } = string.Empty;

        [Description("Optional Title shown above the message")]
        [CommandOption($"--ti|--{nameof(Title)}", isRequired: false)]
        public string? Title { get; set; }

        [Description("Seconds before the toast disappears. 0 = stay until clicked")]
        [CommandOption($"-s|--{nameof(TimeoutSeconds)}", isRequired: false)]
        [DefaultValue(5)]
        public int TimeoutSeconds { get; set; }

        [Description("The alignment of the Text")]
        [CommandOption($"--ta|--{nameof(TextAlignment)}", isRequired: false)]
        [DefaultValue(TextAlignmentType.Left)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public TextAlignmentType TextAlignment { get; set; }

        [Description("The alignment of the Title")]
        [CommandOption($"--tia|--{nameof(TitleAlignment)}", isRequired: false)]
        [DefaultValue(TextAlignmentType.Left)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public TextAlignmentType TitleAlignment { get; set; }

        [Description("Location for where the Toast popup shows")]
        [CommandOption($"-l|--{nameof(Location)}", isRequired: false)]
        [DefaultValue(ToastLocationType.BottomRight)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public ToastLocationType Location { get; set; }

        [Description("Monitor for where the Toast popup shows")]
        [CommandOption($"-m|--{nameof(Monitor)}", isRequired: false)]
        [DefaultValue(ToastMonitorType.Primary)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public ToastMonitorType Monitor { get; set; }

        [Description("Background color for the toast popup")]
        [CommandOption($"--bg|--{nameof(BackgroundColour)}", isRequired: false)]
        public string BackgroundColour { get; set; } = string.Empty;

        [Description("Background Fill style for the toast popup")]
        [CommandOption($"--bgf|--{nameof(BackgroundFillStyle)}", isRequired: false)]
        [DefaultValue(GradientFillStyleType.Solid)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public GradientFillStyleType BackgroundFillStyle { get; set; }

        [Description("Foreground color for the Text")]
        [CommandOption($"--fg|--{nameof(ForegroundColour)}", isRequired: false)]
        [DefaultValue(Constants.Auto)]
        public string ForegroundColour { get; set; } = string.Empty;

        [Description("The transition time for the Window animation, in Milliseconds")]
        [CommandOption($"--tt|--{nameof(TransitionTime)}", isRequired: false)]
        [DefaultValue(1000)]
        public int TransitionTime { get; set; }

        [Description("A Horizontal Indent to use when positioning the popup")]
        [CommandOption($"--hi|--{nameof(HorizontalIndent)}", isRequired: false)]
        [DefaultValue(0.0)]
        public double HorizontalIndent { get; set; }

        [Description($"The width of the popup window, in pixels. Or {Constants.Auto}")]
        [CommandOption($"--ww|--{nameof(WindowWidth)}", isRequired: false)]
        [DefaultValue(Constants.Auto)]
        public string WindowWidth { get; set; } = string.Empty;

        [Description("Optional sound to play when the toast appears: a system sound name (Asterisk, Beep, Exclamation, Hand, Question) or a path to a .wav file")]
        [CommandOption($"--snd|--{nameof(Sound)}", isRequired: false)]
        public string Sound { get; set; } = string.Empty;

        [Description("Font family name for the Text. Must be an installed font")]
        [CommandOption($"--tfn|--{nameof(TextFontName)}", isRequired: false)]
        public string TextFontName { get; set; } = string.Empty;

        [Description("Base font size for the Text, in points")]
        [CommandOption($"--tfs|--{nameof(TextFontSize)}", isRequired: false)]
        [DefaultValue(13.0)]
        public double TextFontSize { get; set; } = 13;

        [Description("Font family name for the Title. Must be an installed font")]
        [CommandOption($"--tifn|--{nameof(TitleFontName)}", isRequired: false)]
        public string TitleFontName { get; set; } = string.Empty;

        [Description("Base font size for the Title, in points")]
        [CommandOption($"--tifs|--{nameof(TitleFontSize)}", isRequired: false)]
        [DefaultValue(14.0)]
        public double TitleFontSize { get; set; }

        [Description("Font style for the Text: Default (this element's own default), Regular, Bold, Italic or BoldItalic")]
        [CommandOption($"--tfst|--{nameof(TextFontStyle)}", isRequired: false)]
        [DefaultValue(FontStyleType.Default)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public FontStyleType TextFontStyle { get; set; }

        [Description("Font style for the Title: Default (this element's own default), Regular, Bold, Italic or BoldItalic")]
        [CommandOption($"--tifst|--{nameof(TitleFontStyle)}", isRequired: false)]
        [DefaultValue(FontStyleType.Default)]
        //[TypeConverter(typeof(AlternateSpellingEnumConverter))]
        public FontStyleType TitleFontStyle { get; set; }

        public void CustomValidate()
        {
            if (string.IsNullOrEmpty(Text))
                throw new ArgumentNullException(nameof(Text));

            if (TimeoutSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "Value must be zero or greater");

            if (TransitionTime < 0)
                throw new ArgumentOutOfRangeException(nameof(TransitionTime), "Value must be zero or greater");

            if (HorizontalIndent < 0)
                throw new ArgumentOutOfRangeException(nameof(HorizontalIndent), "Value must be zero or greater");

            if (!string.IsNullOrWhiteSpace(BackgroundColour) && !ColourParser.TryParsePair(BackgroundColour, out _, out _))
                throw new ArgumentException($"'{BackgroundColour}' is not a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB), optionally two such values separated by ':' for a gradient fill",
                    nameof(BackgroundColour));

            if (BackgroundFillStyle != GradientFillStyleType.Solid && !(BackgroundColour?.Contains(':') ?? false))
                throw new ArgumentException($"BackgroundFillStyle '{BackgroundFillStyle}' requires two colours in BackgroundColour separated by ':' (e.g. Red:Blue)", nameof(BackgroundFillStyle));

            if (!ColourParser.IsAuto(ForegroundColour) && !ColourParser.TryParse(ForegroundColour, out _))
                throw new ArgumentException($"'{ForegroundColour}' is not Auto, a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB)", nameof(ForegroundColour));

            if (!Constants.IsAuto(WindowWidth) && (!double.TryParse(WindowWidth, out var windowWidth) || windowWidth <= 0))
                throw new ArgumentException($"'{WindowWidth}' is not '{Constants.Auto}' or a positive number", nameof(WindowWidth));

            if (!SoundParser.IsValid(Sound))
                throw new ArgumentException($"'{Sound}' is not a known system sound name (Asterisk, Beep, Exclamation, Hand, Question) or an existing .wav file path", nameof(Sound));

            if (!FontParser.IsValid(TextFontName))
                throw new ArgumentException($"'{TextFontName}' is not an installed font", nameof(TextFontName));

            if (TextFontSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(TextFontSize), "Value must be greater than zero");

            if (!FontParser.IsValid(TitleFontName))
                throw new ArgumentException($"'{TitleFontName}' is not an installed font", nameof(TitleFontName));

            if (TitleFontSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(TitleFontSize), "Value must be greater than zero");
        }

        public ToastRequest ToToastRequest()
        {
            return new ToastRequest(
                DynamicText.Resolve(Text),
                Title is null ? null : DynamicText.Resolve(Title),
                TimeoutSeconds,
                Location,
                Monitor,
                BackgroundColour,
                ForegroundColour,
                TextAlignment,
                TitleAlignment,
                TransitionTime,
                HorizontalIndent,
                BackgroundFillStyle,
                WindowWidth,
                Sound,
                TextFontName,
                TextFontSize,
                TitleFontName,
                TitleFontSize,
                TextFontStyle,
                TitleFontStyle
            );
        }
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>()
        {
            { [ "\"Hello World\"" ] },
            { [ "\"The time is now: {now:h:mm tt}\"", "--ww", "Auto" ] },
        };
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
        var request = settings.ToToastRequest();

        return ToastRequestHandler.Handle(request);
    }
}
