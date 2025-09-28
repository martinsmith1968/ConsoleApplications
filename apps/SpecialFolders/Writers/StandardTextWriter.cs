using System.Text;
using SpecialFolders.Configuration;
using SpecialFolders.Writers.Interfaces;

namespace SpecialFolders.Writers;

internal class StandardTextWriter : IWriter
{
    public async Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, Arguments arguments)
    {
        var maxLineNumberWidth = items.Count.ToString().Length;

        var maxIdWidth = items.Max(x => (int)x.Key).ToString().Length;

        var maxNameWith = !arguments.CollapseNames
            ? items.Max(sf => sf.Key.ToString().Length)
            : 0;

        var lineNumber = 0;
        foreach (var sf in items)
        {
            ++lineNumber;

            var line = new StringBuilder();

            if (arguments.ShowLineNumbers)
                line.Append(lineNumber.ToString().PadLeft(maxLineNumberWidth));

            if (arguments.ShowFolderId)
            {
                if (line.Length > 0)
                    line.Append(' ');
                line.Append(((int)sf.Key).ToString().PadRight(maxIdWidth));
            }

            if (line.Length > 0)
                line.Append(' ');

            line.Append(
                arguments.CollapseNames
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
