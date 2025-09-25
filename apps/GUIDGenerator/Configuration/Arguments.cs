using System.ComponentModel;
using ConsoleApplications.Common.Configuration;
using Ookii.CommandLine;

namespace GUIDGenerator.Configuration;

[GeneratedParser]
public partial class Arguments
{
    [Alias("n")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 1)]
    [Description("How many GUID values to generate")]
    public int Count { get; set; }

    [Alias("p")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "")]
    [Description("The text prefix to use after GUID values")]
    public string Prefix { get; set; } = "";

    [Alias("s")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "\r\n")]
    [Description("The text suffix to use after GUID values")]
    public string Suffix { get; set; } = "";

    [Alias("fmt")]
    [CommandLineArgument(IsRequired = false, DefaultValue = "")]
    [Description("The format to use when generating GUID values")]
    public string GUIDFormat { get; set; } = "";

    [Alias("conv")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDCaseConversionType.None)]
    [Description("What Text case conversion should be applied to GUID values")]
    public GUIDCaseConversionType CaseConversionType { get; set; } = GUIDCaseConversionType.None;

    [Alias("c")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    [Description("Copy the genearated values to the Clipboard ?")]
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
