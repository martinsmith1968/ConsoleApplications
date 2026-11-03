using System.ComponentModel;
using Ookii.CommandLine;

namespace TomlFileHandler.Commands;

public abstract class BaseTOMLCommand : BaseCommand
{
    [CommandLineArgument(IsRequired = true)]
    [Description("The TOML file name to read")]
    [Alias("f")]
    public string FileName { get; set; } = null!;

    public FileInfo FileNameInfo => new(FileName);

    public override void Validate()
    {
        if (!FileNameInfo.Exists)
            throw new FileNotFoundException($"{nameof(FileName)} does not exist");
    }
}
