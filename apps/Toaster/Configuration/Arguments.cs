using System.ComponentModel;
using System.Drawing;
using ConsoleApplications.Common.Application;
using Ookii.CommandLine;

namespace Toaster.Configuration;

public class Arguments : IValidatableArguments
{
    [Alias("t")]
    [Description("The text to display")]
    [CommandLineArgument(IsRequired = true, IsPositional = true, Position = 1)]
    public string Text { get; set; } = string.Empty;

    [Alias("i")]
    [Description("Optional Title shown above the message")]
    [CommandLineArgument(IsRequired = false)]
    public string? Title { get; set; }

    [Alias("s")]
    [Description("Seconds before the toast disappears. 0 = stay until clicked")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 5)]
    public int TimeoutSeconds { get; set; }

    [Alias("a")]
    [Description("The alignment of the Text")]
    [CommandLineArgument(IsRequired = false, DefaultValue = TextAlignmentType.Left)]
    public TextAlignmentType TextAlignment { get; set; }

    [Alias("g")]
    [Description("The alignment of the Title")]
    [CommandLineArgument(IsRequired = false, DefaultValue = TextAlignmentType.Left)]
    public TextAlignmentType TitleAlignment { get; set; }

    [Alias("l")]
    [Description("Location for where the Toast popup shows")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastLocationType.BottomRight)]
    public ToastLocationType Location { get; set; }

    [Alias("o")]
    [Description("Monitor for where the Toast popup show")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastMonitorType.Primary)]
    public ToastMonitorType Monitor { get; set; }

    [Alias("b")]
    [Description("Background color for the toast popup")]
    [CommandLineArgument(IsRequired = false)]
    public string BackgroundColour { get; set; } = string.Empty;

    [Alias("f")]
    [Description("Foreground color for the Message Text")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "Auto")]
    public string ForegroundColour { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Text))
            throw new ArgumentNullException(nameof(Text));

        if (TimeoutSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "Value must be zero or greater");

        if (!string.IsNullOrWhiteSpace(BackgroundColour) && !ColourParser.TryParse(BackgroundColour, out _))
            throw new ArgumentException($"'{BackgroundColour}' is not a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB)", nameof(BackgroundColour));

        if (!ColourParser.IsAuto(ForegroundColour) && !ColourParser.TryParse(ForegroundColour, out _))
            throw new ArgumentException($"'{ForegroundColour}' is not Auto, a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB)", nameof(ForegroundColour));
    }
}
