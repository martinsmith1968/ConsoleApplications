using System.ComponentModel;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace DisplaySettings.Configuration;

public enum CommandType
{
    View = 1,
}

[GeneratedParser]
public partial class Arguments
{
    [Description("The command to execute")]
    [CommandLineArgument(IsRequired = false, Position = 1, DefaultValue = CommandType.View)]
    public CommandType Command { get; set; }


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
