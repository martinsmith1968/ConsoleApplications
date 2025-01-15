using Ookii.CommandLine.Commands;
using Ookii.CommandLine;
using System.ComponentModel;

namespace TomlFileHandler.Commands;

[GeneratedParser]
[Command("DELETE")]
[Description("Delete a Section or Value from a TOML file")]
public partial class DeleteCommand : BaseTOMLSectionCommand
{
    // TODO
    public override void Execute()
    {
        throw new NotImplementedException();
    }
}
