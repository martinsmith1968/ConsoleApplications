using System.ComponentModel;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;

namespace TomlFileHandler.Commands;

[GeneratedParser]
[Command("EXPORT")]
[Description("Export a TOML file in a different format")]
public partial class ExportCommand : BaseTOMLCommand
{
    public enum ExportFormatType
    {
        JSON=1,
        INI
    }

    [CommandLineArgument(IsRequired = true)]
    public ExportFormatType FormatType { get; set; }

    public override void Validate()
    {
        base.Validate();

        if (!Enum.GetValues<ExportFormatType>().Contains(FormatType))
            throw new ArgumentOutOfRangeException(nameof(FormatType), $"Unknown Export Format - allowed values: {string.Join(",", Enum.GetNames<ExportFormatType>())}");
    }

    // TODO
    public override void Execute()
    {
        throw new NotImplementedException();
    }
}
