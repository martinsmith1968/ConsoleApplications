using System.ComponentModel;
using ConsoleApplications.Common.CommandLine;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace PauseN.Configuration;

[GeneratedParser]
public partial class Arguments : CustomParseOptionsOverride
{
    public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSeconds#]";

    [Description("How long to wait (in seconds) before continuing")]
    [CommandLineArgument(IsRequired = false, Position = 1, DefaultValue = 10)]
    public int TimeoutSeconds { get; set; }

    [Description("The text to display")]
    [Alias("t")]
    [CommandLineArgument(IsRequired = false, DefaultValue = $"Press any key to continue (or wait {PlaceHolder_TimeoutSeconds} seconds)")]
    public string Text { get; set; } = "";

    [Description("How long to wait (in seconds) before continuing")]
    [Alias("s")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 100)]
    public int SleepMilliseconds { get; set; }

    public void Validate()
    {
        if (TimeSpan.FromMilliseconds(SleepMilliseconds) > TimeSpan.FromSeconds(TimeoutSeconds))
            throw new Exception($"{nameof(SleepMilliseconds)} exceeds permitted value - is greater than {nameof(TimeoutSeconds)}");
    }

    public string DisplayText => Text
        .Replace(PlaceHolder_TimeoutSeconds, TimeoutSeconds.ToString())
        ;



    public static CustomParseOptions Options => new()
    {
        AutoHelpArgument = true,
        AutoVersionArgument = true,
        //DuplicateArguments     = ErrorMode.Error,
        //Mode                   = ParsingMode.LongShort,
        //LongArgumentNamePrefix = "--",
        //ArgumentNamePrefixes   = new[] { "/", "-" },
        ShowUsageOnError = UsageHelpRequest.Full,
        //NameValueSeparator     = ':',
        //Error                  = Console.Error,
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError()),
    };
}
