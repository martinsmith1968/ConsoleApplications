using System.ComponentModel;
using CommandLine;

namespace CommandLineParser.Spike.SingleCommand.Configuration;

[Description("A single command app")]
internal partial class Arguments
{
    [Option('n', Required = true)]
    [Description("A required positional argument.")]
    public required IEnumerable<string> Name { get; set; }

    [Option('d')]
    [Description("An argument that can only be supplied by name.")]
    public DateTime BirthDate { get; set; }

    [Option('o', Default = false)]
    [Description("A switch argument, which doesn't require a value.")]
    public bool SwitchOn { get; set; }

    [Option('f', Default = true)]
    [Description("A switch argument, which doesn't require a value.")]
    public bool SwitchOff { get; set; }
}
