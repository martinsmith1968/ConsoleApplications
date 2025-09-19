using System.ComponentModel;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace PrintFormat.Configuration;

public class Arguments
{
    public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSeconds#]";

    [Description("The Format string to use")]
    [CommandLineArgument(IsRequired = true, Position = 1)]
    public string? Format { get; set; }

    [Description("The format arguments")]
    [CommandLineArgument(IsRequired = true, Position = 2)]
    public string[] FormatArguments { get; set; } = {};

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Format))
            throw new ArgumentNullException(nameof(Format));
        if (!FormatArguments.Any())
            throw new ArgumentNullException(nameof(FormatArguments));
    }

    public static ParseOptions Options => new()
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
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError(), true),
    };
}
