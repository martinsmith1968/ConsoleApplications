using System.ComponentModel;
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
    [Description("How many GUID values to generate")]
    public int Count { get; set; }

    [Alias("t")]
    [CommandLineArgument(IsRequired = false)]
    [Description("The text lines to use")]
    public string[] Text { get; set; } = [ ];

    [Alias("m")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDGenerationModeType.Standard)]
    [Description("TODO")]
    public GUIDGenerationModeType Mode { get; set; }

    [Alias("a")]
    [CommandLineArgument(IsRequired = false, DefaultValue = GUIDCaseConversionType.None)]
    [Description("TODO")]
    public GUIDCaseConversionType CaseConversionType { get; set; }

    [Alias("f")]
    [Alias("fmt")]
    [CommandLineArgument(IsRequired = false)]
    [Description("TODO")]
    public string? Format { get; set; }

    [Alias("d")]
    [Alias("display")]
    [CommandLineArgument(IsRequired = false, DefaultValue = FORMAT_PLACEHOLDER_GUID)]
    [Description("TODO")]
    public string? DisplayText { get; set; }

    [Alias("e")]
    [CommandLineArgument(IsRequired = false)]
    [Description("TODO")]
    public string Delimiter { get; set; } = Environment.NewLine;

    [Alias("h")]
    [CommandLineArgument(IsRequired = false, DefaultValue = true)]
    [Description("TODO")]
    public bool ShowHeader { get; set; }

    [Alias("b")]
    [Alias("copy")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    [Description("TODO")]
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
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError()),
    };
}
