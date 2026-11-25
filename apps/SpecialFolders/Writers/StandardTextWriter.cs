using System.Text;
using SpecialFolders.Commands;
using SpecialFolders.Writers.Interfaces;

namespace SpecialFolders.Writers;

internal class StandardTextWriter : IWriter
{
    public async Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, SpecialFoldersCommand.Settings settings)
    {
        var maxLineNumberWidth = items.Count.ToString().Length;

        var maxIdWidth = items.Max(x => (int)x.Key).ToString().Length;

        var maxNameWith = !settings.CollapseNames
            ? items.Max(sf => sf.Key.ToString().Length)
            : 0;

        var lineNumber = 0;
        foreach (var sf in items)
        {
            ++lineNumber;

            var line = new StringBuilder();

            if (settings.ShowLineNumbers)
                line.Append(lineNumber.ToString().PadLeft(maxLineNumberWidth));

            if (settings.ShowFolderId)
            {
                if (line.Length > 0)
                    line.Append(' ');
                line.Append(((int)sf.Key).ToString().PadRight(maxIdWidth));
            }

            if (line.Length > 0)
                line.Append(' ');

            line.Append(
                settings.CollapseNames
                    ? sf.Key.ToString()
                    : sf.Key.ToString().PadRight(maxNameWith)
            );

            if (line.Length > 0)
                line.Append(' ');

            line.Append(sf.Value);

            await Console.Out.WriteLineAsync(line.ToString());
        }
    }
}
