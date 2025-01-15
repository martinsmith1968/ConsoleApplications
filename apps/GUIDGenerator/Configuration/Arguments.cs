using ConsoleApplications.Common.Application;
using GUIDGenerator.Services.Types;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace GUIDGenerator.Configuration;

[GeneratedParser]
public partial class Arguments : IValidatableArguments
{
    public const string FORMAT_PLACEHOLDER_GUID_PREFIX = "{GUID";
    public const string FORMAT_PLACEHOLDER_INDEX_PREFIX = "{INDEX";

    public const string FORMAT_PLACEHOLDER_GUID  = FORMAT_PLACEHOLDER_GUID_PREFIX + "}";
    public const string FORMAT_PLACEHOLDER_INDEX = FORMAT_PLACEHOLDER_INDEX_PREFIX + "}";

    [Alias("c")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 1)]
    public int Count { get; set; }

    [Alias("t")]
    [CommandLineArgument(IsRequired = false)]
    public string[] Text { get; set; }

    [Alias("m")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDGenerationModeType.Standard)]
    public GUIDGenerationModeType Mode { get; set; }

    [Alias("a")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDCaseConversionType.None)]
    public GUIDCaseConversionType CaseConversionType { get; set; }

    [Alias("f")]
    [Alias("fmt")]
    [CommandLineArgument(IsRequired = false)]
    public string Format { get; set; }

    [Alias("d")]
    [Alias("display")]
    [CommandLineArgument(IsRequired = false, DefaultValue = FORMAT_PLACEHOLDER_GUID)]
    public string DisplayText { get; set; }

    [Alias("e")]
    [CommandLineArgument(IsRequired = false)]
    public string Delimiter { get; set; } = Environment.NewLine;

    [Alias("h")]
    [CommandLineArgument(IsRequired = false, DefaultValue = true)]
    public bool ShowHeader { get; set; }

    [Alias("b")]
    [Alias("copy")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool CopyToClipboard { get; set; }

    public void Validate()
    {

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
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError(), true),
    };
}
