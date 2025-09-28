using SpecialFolders.Configuration;
using SpecialFolders.Writers.Interfaces;
using Spectre.Console;

namespace SpecialFolders.Writers;

/// <summary>
///
/// </summary>
/// <seealso cref="SpecialFolders.Writers.Interfaces.IWriter" />
/// <remarks>
/// See: https://spectreconsole.net/widgets/grid
/// </remarks>
internal class GridWriter : IWriter
{
    public Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, Arguments arguments)
    {
        var grid = new Grid();

        grid.AddColumn();
        grid.AddColumn();

        foreach (var sf in items)
        {
            grid.AddRow(sf.Key.ToString(), sf.Value);
        }

        AnsiConsole.Write(grid);

        return Task.CompletedTask;
    }
}
