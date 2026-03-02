using System.ComponentModel;

namespace Ookii.CommandLine.Spike.SingleCommand.Configuration;

[GeneratedParser]
[Description("A single command app")]
internal partial class Arguments
{
    [CommandLineArgument(IsPositional = true)]
    [Description("A required positional argument.")]
    public required string[] Name { get; set; }

    [CommandLineArgument(ShortName = 'd')]
    [Description("An argument that can only be supplied by name.")]
    public DateTime BirthDate { get; set; }

    [CommandLineArgument(DefaultValue = false, ShortName = 'o')]
    [Description("A switch argument, which doesn't require a value.")]
    public bool SwitchOn { get; set; }

    [CommandLineArgument(DefaultValue = true, ShortName = 'f')]
    [Description("A switch argument, which doesn't require a value.")]
    public bool SwitchOff { get; set; }
}
