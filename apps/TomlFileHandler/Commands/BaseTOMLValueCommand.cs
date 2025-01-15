using Ookii.CommandLine;
using System.ComponentModel;

namespace TomlFileHandler.Commands;

public abstract class BaseTOMLValueCommand : BaseTOMLSectionCommand
{
    [CommandLineArgument(IsRequired = true)]
    [Description("The TOML Key name to read")]
    [Alias("n")]
    public string KeyName { get; set; } = null!;

    [CommandLineArgument(IsRequired = false)]
    [Description("The TOML Key value index to access (For Array values)")]
    [Alias("i")]
    public int? ValueIndex { get; set; }

    public bool HasValueIndex => ValueIndex.HasValue;
}
