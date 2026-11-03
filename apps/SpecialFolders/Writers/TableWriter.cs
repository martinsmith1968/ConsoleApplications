using SpecialFolders.Configuration;
using SpecialFolders.Writers.Interfaces;
using Spectre.Console;

namespace SpecialFolders.Writers;

/// <summary>
///
/// </summary>
/// <seealso cref="SpecialFolders.Writers.Interfaces.IWriter" />
/// <remarks>
/// See: https://spectreconsole.net/widgets/table
/// </remarks>
internal class TableWriter : IWriter
{
    public Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, Arguments arguments)
    {
        var table = new Table();

        if (arguments.ShowLineNumbers)
        {
            table.AddColumn("#", x => x.Alignment = Justify.Right);
        }
        if (arguments.ShowFolderId)
        {
            table.AddColumn("Id");
        }
        table.AddColumn("Name");
        table.AddColumn("Location");

        foreach (var sf in items)
        {
            var data = new List<string>();

            if (arguments.ShowLineNumbers)
            {
                data.Add((table.Rows.Count + 1).ToString());
            }

            if (arguments.ShowFolderId)
            {
                data.Add(((int)sf.Key).ToString());
            }
            data.Add(sf.Key.ToString());
            data.Add(sf.Value);

            table.AddRow(data.ToArray());
        }

        AnsiConsole.Write(table);

        return Task.CompletedTask;
    }
}
