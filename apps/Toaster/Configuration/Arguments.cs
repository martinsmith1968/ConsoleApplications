using System.ComponentModel;
using ConsoleApplications.Common.Application;
using Ookii.CommandLine;

namespace Toaster.Configuration;

public class Arguments : IValidatableArguments
{
    [Alias("t")]
    [Description("The text to display")]
    [CommandLineArgument(IsRequired = true, IsPositional = true, Position = 1)]
    public string Text { get; set; } = string.Empty;

    [Alias("ti")]
    [Description("Optional Title shown above the message")]
    [CommandLineArgument(IsRequired = false)]
    public string? Title { get; set; }

    [Alias("s")]
    [Description("Seconds before the toast disappears. 0 = stay until clicked")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 5)]
    public int TimeoutSeconds { get; set; }

    [Alias("ta")]
    [Description("The alignment of the Text")]
    [CommandLineArgument(IsRequired = false, DefaultValue = TextAlignmentType.Left)]
    public TextAlignmentType TextAlignment { get; set; }

    [Alias("tia")]
    [Description("The alignment of the Title")]
    [CommandLineArgument(IsRequired = false, DefaultValue = TextAlignmentType.Left)]
    public TextAlignmentType TitleAlignment { get; set; }

    [Alias("l")]
    [Description("Location for where the Toast popup shows")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastLocationType.BottomRight)]
    public ToastLocationType Location { get; set; }

    [Alias("m")]
    [Description("Monitor for where the Toast popup show")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastMonitorType.Primary)]
    public ToastMonitorType Monitor { get; set; }

    [Alias("bg")]
    [Description("Background color for the toast popup")]
    [CommandLineArgument(IsRequired = false)]
    public string BackgroundColour { get; set; } = string.Empty;

    [Alias("bgf")]
    [Description("Background Fill style for the toast popup")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GradientFillStyleType.Solid)]
    public GradientFillStyleType BackgroundFillStyle { get; set; }

    [Alias("fg")]
    [Description("Foreground color for the Message Text")]
    [CommandLineArgument(IsRequired = false, DefaultValue = Constants.Auto)]
    public string ForegroundColour { get; set; } = string.Empty;

    [Alias("tt")]
    [Description("The transition time for the Window animation, in Milliseconds")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 1000)]
    public int TransitionTimeMilliseconds { get; set; }

    [Alias("hi")]
    [Description("A Horizontal Indent to use when positioning the popup")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 0)]
    public double HorizontalIndent { get; set; }

    [Alias("w")]
    [Description($"The width of the popup window, in pixels. Or {Constants.Auto}")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastWindow.DefaultWidth)]
    public string WindowWidth { get; set; } = string.Empty;

    [Alias("snd")]
    [Description("Optional sound to play when the toast appears: a system sound name (Asterisk, Beep, Exclamation, Hand, Question) or a path to a .wav file")]
    [CommandLineArgument(IsRequired = false)]
    public string Sound { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Text))
            throw new ArgumentNullException(nameof(Text));

        if (TimeoutSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "Value must be zero or greater");

        if (TransitionTimeMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(TransitionTimeMilliseconds), "Value must be zero or greater");

        if (HorizontalIndent < 0)
            throw new ArgumentOutOfRangeException(nameof(HorizontalIndent), "Value must be zero or greater");

        if (!string.IsNullOrWhiteSpace(BackgroundColour) && !ColourParser.TryParsePair(BackgroundColour, out _, out _))
            throw new ArgumentException($"'{BackgroundColour}' is not a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB), optionally two such values separated by ':' for a gradient fill", nameof(BackgroundColour));

        if (BackgroundFillStyle != GradientFillStyleType.Solid && !(BackgroundColour?.Contains(':') ?? false))
            throw new ArgumentException($"BackgroundFillStyle '{BackgroundFillStyle}' requires two colours in BackgroundColour separated by ':' (e.g. Red:Blue)", nameof(BackgroundFillStyle));

        if (!ColourParser.IsAuto(ForegroundColour) && !ColourParser.TryParse(ForegroundColour, out _))
            throw new ArgumentException($"'{ForegroundColour}' is not Auto, a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB)", nameof(ForegroundColour));

        if (!Constants.IsAuto(WindowWidth) && (!double.TryParse(WindowWidth, out var windowWidth) || windowWidth <= 0))
            throw new ArgumentException($"'{WindowWidth}' is not '{Constants.Auto}' or a positive number", nameof(WindowWidth));

        if (!SoundParser.IsValid(Sound))
            throw new ArgumentException($"'{Sound}' is not a known system sound name (Asterisk, Beep, Exclamation, Hand, Question) or an existing .wav file path", nameof(Sound));
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
            TransitionTimeMilliseconds,
            HorizontalIndent,
            BackgroundFillStyle,
            WindowWidth,
            Sound
        );
    }
}
