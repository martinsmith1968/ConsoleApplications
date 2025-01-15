using ConsoleApplications.Common.Application;
using Ookii.CommandLine;

namespace Toaster.Configuration;

public class Arguments : IValidatableArguments
{
    [Alias("t")]
    [CommandLineArgument(IsRequired = true, IsPositional = true, Position = 1)]
    public string Text { get; set; }

    [Alias("i")]
    [CommandLineArgument(IsRequired = false)]
    public string Title { get; set; }



    [CommandLineArgument(IsRequired = false, DefaultValue = 5)]
    public int TimeoutSeconds { get; set; }
















    public void Validate()
    {
        if (string.IsNullOrEmpty(Text))
            throw new ArgumentNullException(nameof(Text));

        if (TimeoutSeconds < 0)
            throw new ArgumentOutOfRangeException(nameof(TimeoutSeconds), "Value must be a positive number");
    }
}
