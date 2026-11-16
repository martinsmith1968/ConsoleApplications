using System.ComponentModel;
using System.Drawing;
using ConsoleApplications.Common.Application;
using Ookii.CommandLine;

namespace Toaster.Configuration;

public class Arguments : IValidatableArguments
{
    [Alias("m")]
    [Description("The message text to display")]
    [CommandLineArgument(IsRequired = true, IsPositional = true, Position = 1)]
    public string MessageText { get; set; } = string.Empty;

    [Alias("i")]
    [Description("Optional title shown above the message")]
    [CommandLineArgument(IsRequired = false)]
    public string? Title { get; set; }

    [Alias("s")]
    [Description("Seconds before the toast disappears. 0 = stay until clicked")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 5)]
    public int TimeoutSeconds { get; set; }

    [Alias("l")]
    [Description("Location for where the Toast popup shows")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastLocationType.BottomRight)]
    public ToastLocationType Location { get; set; }

    [Alias("o")]
    [Description("Monitor for where the Toast popup show")]
    [CommandLineArgument(IsRequired = false, DefaultValue = ToastMonitorType.Primary)]
    public ToastMonitorType Monitor { get; set; }

    [Alias("b")]
    [Description("Background color for the toast")]
    [CommandLineArgument(IsRequired = false)]
    public string BackgroundColour { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrEmpty(MessageText))
            throw new ArgumentNullException(nameof(MessageText));

        if (TimeoutSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "Value must be zero or greater");

        if (!string.IsNullOrWhiteSpace(BackgroundColour) && !ColourParser.TryParse(BackgroundColour, out _))
            throw new ArgumentException($"'{BackgroundColour}' is not a known colour name or a # value (#RGB, #RRGGBB, #AARRGGBB)", nameof(BackgroundColour));
    }
}
