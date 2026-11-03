using System.ComponentModel;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using TomlFileHandler.Services;

namespace TomlFileHandler.Commands;

[GeneratedParser]
[Command("SET")]
[Description("Set a value in a TOML file")]
internal partial class SetCommand : BaseTOMLValueCommand
{
    [CommandLineArgument(IsRequired = true)]
    [Description("The value to set")]
    [Alias("v")]
    public string Value { get; set; }

    public override void Validate()
    {
        base.Validate();

        if (ValueIndex is < 0)
            throw new ArgumentOutOfRangeException(nameof(ValueIndex), "Must be 0 or greater");
    }

    public override void Execute()
    {
        var table = TommyTOMLFileService.ReadFile(FileNameInfo.FullName);

        TommyTOMLFileService.SetTomlValue(table, SectionName, KeyName, ValueIndex, Value);

        TommyTOMLFileService.WriteFile(table, FileNameInfo.FullName);
    }
}
