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

        if (arguments.ShowLineNumbers)
        {
            grid.AddColumn(new GridColumn() { Alignment = Justify.Right });
        }
        if (arguments.ShowFolderId)
        {
            grid.AddColumn();
        }
        grid.AddColumn();
        grid.AddColumn();

        grid.AddRow(
            new Text("#", new Style(Color.Green)),
            new Text("Id", new Style(Color.Green)),
            new Text("Name", new Style(Color.Green)),
            new Text("Location", new Style(Color.Green))
        );

        var lineNumber = 0;
        foreach (var sf in items)
        {
            var data = new List<string>();

            if (arguments.ShowLineNumbers)
            {
                data.Add((++lineNumber).ToString());
            }

            if (arguments.ShowFolderId)
            {
                data.Add(((int)sf.Key).ToString());
            }
            data.Add(sf.Key.ToString());
            data.Add(sf.Value);

            grid.AddRow(data.ToArray());
        }

        AnsiConsole.Write(grid);

        return Task.CompletedTask;
    }
}
