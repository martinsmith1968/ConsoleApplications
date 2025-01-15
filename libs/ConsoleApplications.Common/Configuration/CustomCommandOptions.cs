using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using Ookii.CommandLine.Terminal;

namespace ConsoleApplications.Common.Configuration;

public  class CustomCommandOptions : CommandOptions
{
    public CustomCommandOptions()
    {
        Error               = Console.Error;
        AutoHelpArgument    = true;
        AutoVersionArgument = true;
        AutoVersionCommand  = true;
        DuplicateArguments  = ErrorMode.Error;
        ErrorColor          = TextFormat.BrightForegroundRed;
        ShowUsageOnError    = UsageHelpRequest.SyntaxOnly;
        WarningColor        = TextFormat.BrightForegroundYellow;
        UsageWriter         = new CustomUsageWriter();
    }

    public CustomCommandOptions(Action<CustomCommandOptions> configurator)
        : this()
    {
        configurator?.Invoke(this);
    }
}
