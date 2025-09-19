using ConsoleApplications.Common.Configuration;
using Ookii.CommandLine;

namespace GUIDGenerator.Configuration;

public class Arguments
{
    [Alias("n")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 1)]
    public int Count { get; set; }

    [Alias("p")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "")]
    public string Prefix { get; set; } = "";

    [Alias("s")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "\r\n")]
    public string Suffix { get; set; } = "";

    [Alias("fmt")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "")]
    public string GUIDFormat { get; set; } = "";

    [Alias("conv")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDCaseConversionType.None)]
    public GUIDCaseConversionType CaseConversionType { get; set; }

    [Alias("c")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool CopyToClipboard { get; set; }

    public void Validate()
    {
        try
        {
            _ = Guid.NewGuid().ToString(GUIDFormat);
        }
        catch
        {
            throw new Exception($"{nameof(GUIDFormat)} is an unknown or invalid Format : {GUIDFormat}");
        }
    }

    public static ParseOptions Options => new()
    {
        AutoHelpArgument       = true,
        AutoVersionArgument    = true,
        //DuplicateArguments     = ErrorMode.Error,
        //Mode                   = ParsingMode.LongShort,
        //LongArgumentNamePrefix = "--",
        //ArgumentNamePrefixes   = new[] { "/", "-" },
        ShowUsageOnError       = UsageHelpRequest.Full,
        //NameValueSeparator     = ':',
        //Error                  = Console.Error,
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError()),
    };
}
