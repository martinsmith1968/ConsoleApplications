using Ookii.CommandLine;
using SpecialFolders.Configuration;
using SpecialFolders.Configuration.Types;

namespace SpecialFolders;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args, Arguments.Options)
                            ?? throw new Exception("Unable to Parse Command Line");
            arguments.Validate();

            await Process(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }

    private static async Task Process(Arguments arguments)
    {
        var specialFolders = Enum.GetValues<Environment.SpecialFolder>()
            .Distinct()
            .ToDictionary(
                sf => sf,
                Environment.GetFolderPath
            );

        var specialFoldersList = specialFolders
                .Where(sf => string.IsNullOrWhiteSpace(arguments.Name?.ToString())
                             ||  sf.Key == arguments.Name
                    )
                .Where(sf => !arguments.HideEmptyLocation || !string.IsNullOrWhiteSpace(sf.Value))
                .ToList();

        specialFoldersList = arguments.SortBy switch
        {
            SortKey.Name => specialFoldersList.OrderBy(sf => sf.Key.ToString()).ToList(),
            SortKey.Location => specialFoldersList.OrderBy(sf => sf.Value).ToList(),
            _ => specialFoldersList
        };

        var maxNameWith = !arguments.CollapseNames
            ? specialFoldersList.Max(sf => sf.Key.ToString().Length)
            : 0;

        var locationsText = string.Empty;

        foreach (var sf in specialFoldersList)
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
