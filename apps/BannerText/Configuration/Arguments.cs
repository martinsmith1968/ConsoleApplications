using System.ComponentModel;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace BannerText.Configuration;

public class Arguments
{
    [Description("The text line(s) to display")]
    [CommandLineArgument(IsRequired = true, Position = 1)]
    public string[] Text { get; set; }

    [Description("How to align the text line(s)")]
    [ValueDescription(TextAlignmentTypeExtensions.AllValues)]
    [CommandLineArgument(IsRequired = false, DefaultValue = TextAlignmentType.Left)]
    public TextAlignmentType Alignment { get; set; }

    [Alias("hlc")]
    [Description("The character to ue for Header lines")]
    [CommandLineArgument(IsRequired = false, DefaultValue = '*')]
    public char HeaderLineChar { get; set; }

    [Alias("hln")]
    [Description("The number of Header lines to generate")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 1)]
    public int HeaderLineCount { get; set; }

    [Alias("blb")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 0)]
    public int BlankLinesBefore { get; set; }

    [Alias("bla")]
    [CommandLineArgument(IsRequired = false, DefaultValue = 0)]
    public int BlankLinesAfter { get; set; }

    public void Validate()
    {
        if (BlankLinesBefore < 0)
            throw new ArgumentException(nameof(BlankLinesBefore), $"{nameof(BlankLinesBefore)} must be 0 or greater");
        if (BlankLinesAfter < 0)
            throw new ArgumentException(nameof(BlankLinesAfter), $"{nameof(BlankLinesAfter)} must be 0 or greater");
    }

    private static UsageWriter CustomUsageWriter()
    {
        var usageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError(), true);

        usageWriter.IncludeAliasInDescription = true;
        usageWriter.IncludeApplicationDescription = true;
        usageWriter.IncludeApplicationDescriptionBeforeCommandList = true;
        usageWriter.IncludeCommandHelpInstruction = true;
        usageWriter.IncludeCommandAliasInCommandList = true;
        usageWriter.IncludeDefaultValueInDescription = true;
        usageWriter.IncludeValidatorsInDescription = true;

        return usageWriter;
    }

    public static ParseOptions Options => new()
    {
        AutoHelpArgument = true,
        AutoVersionArgument = true,
        DuplicateArguments     = ErrorMode.Error,
        //Mode                   = ParsingMode.LongShort,
        //LongArgumentNamePrefix = "--",
        //ArgumentNamePrefixes   = new[] { "/", "-" },
        ShowUsageOnError = UsageHelpRequest.Full,
        //NameValueSeparator     = ':',
        Error                  = Console.Error,
        UsageWriter = CustomUsageWriter(),
    };
}
