using Ookii.CommandLine;
using System.ComponentModel;

namespace TomlFileHandler.Commands;

public abstract class BaseTOMLSectionCommand : BaseTOMLCommand
{
    [CommandLineArgument(IsRequired = false)]
    [Description("The TOML section to read a value from")]
    [Alias("s")]
    public string SectionName { get; set; } = null!;

    public bool HasSectionName => !string.IsNullOrWhiteSpace(SectionName);
}
