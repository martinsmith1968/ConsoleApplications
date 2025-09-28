using SpecialFolders.Configuration;
using SpecialFolders.Writers.Interfaces;

namespace SpecialFolders.Writers;

internal class StandardTextWriter : IWriter
{
    public async Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, Arguments arguments)
    {
        var maxNameWith = !arguments.CollapseNames
            ? items.Max(sf => sf.Key.ToString().Length)
            : 0;

        var locationsText = string.Empty;


        foreach (var sf in items)
        {
            if (!string.IsNullOrWhiteSpace(locationsText))
                locationsText += Environment.NewLine;
            locationsText += Environment.GetFolderPath(sf.Key);

            var sfName = !arguments.CollapseNames
                ? sf.Key.ToString().PadRight(maxNameWith)
                : sf.Key.ToString();

            await Console.Out.WriteLineAsync($"{sfName} : {sf.Value}");
        }
    }
}
