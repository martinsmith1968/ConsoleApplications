using System.ComponentModel;
using Ookii.CommandLine;
using SpecialFolders.Configuration.Types;

// ReSharper disable InconsistentNaming

namespace SpecialFolders.Configuration;

public class Arguments
{
    public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSeconds#]";

    [Description("The Special Folder to show")]
    [CommandLineArgument(IsRequired = false, Position = 1, DefaultValue = null)]
    public Environment.SpecialFolder? Name { get; set; }

    [Description("Collapse the Folder Names (Do not align)")]
    [Alias("c")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool CollapseNames { get; set; }

    [Description("Hide the Item with an empty folder location")]
    [Alias("h")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool HideEmptyLocation { get; set; }

    [Description("Sort the results")]
    [Alias("s")]
    [CommandLineArgument(IsRequired = false, DefaultValue = SortKey.None)]
    public SortKey SortBy { get; set; }

    public void Validate()
    {
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
