using System.ComponentModel;
using Ookii.CommandLine;

// ReSharper disable InconsistentNaming

namespace SpecialFolders.Configuration;

public class Arguments
{
    public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSeconds#]";

    [Description("The Special Folder to show")]
    [CommandLineArgument(IsRequired = false, Position = 1, DefaultValue = null)]
    public Environment.SpecialFolder? FolderName { get; set; }

    [Description("Copy the Folder Location(s) to the clipboard")]
    [Alias("c")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool CopyToClipboard { get; set; }

    [Description("Align the Folder Names")]
    [Alias("a")]
    [CommandLineArgument(IsRequired = false, DefaultValue = true)]
    public bool AlignNames { get; set; }

    [Description("Hide the Item with an empty folder location")]
    [Alias("h")]
    [CommandLineArgument(IsRequired = false, DefaultValue = false)]
    public bool HideEmptyLocation { get; set; }

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
        UsageWriter = new UsageWriter(LineWrappingTextWriter.ForConsoleError(), true),
    };
}
