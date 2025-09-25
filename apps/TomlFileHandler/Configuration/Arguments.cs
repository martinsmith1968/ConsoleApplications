using System.ComponentModel;
using Ookii.CommandLine;

namespace TomlFileHandler.Configuration;

public enum CommandType
{
    Read,
    Write
}

public class Arguments
{
    [Description("The Command to execute")]
    [CommandLineArgument(Position = 1, IsRequired = true)]
    public CommandType Command { get; set; }

    [Description("The file name to access")]
    [Alias("f")]
    [CommandLineArgument(IsRequired = true)]
    public string FileName { get; set; } = string.Empty;

    [Description("The Section name to access")]
    [Alias("s")]
    [CommandLineArgument(IsRequired = false)]
    public string SectionName { get; set; } = string.Empty;

    [Description("The Key name to access")]
    [Alias("k")]
    [CommandLineArgument(IsRequired = true)]
    public string KeyName { get; set; } = string.Empty;

    [Description("The Value to get / set")]
    [Alias("v")]
    [CommandLineArgument(IsRequired = false)]
    public string Value { get; set; } = string.Empty;

    public bool HasSectionName => !string.IsNullOrWhiteSpace(SectionName);

    public FileInfo? FileInfo
    {
        get
        {
            try
            {
                return new FileInfo(FileName);
            }
            catch
            {
                return null;
            }
        }
    }

    public void Validate()
    {
        if (FileInfo == null)
            throw new ArgumentException($"Invalid FileName: {FileName}", nameof(FileName));
        if (Command == CommandType.Write && string.IsNullOrEmpty(Value))
            throw new ArgumentException($"{nameof(Value)} must to be specified for command: {Command}", Value);
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
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError()),
    };
}
