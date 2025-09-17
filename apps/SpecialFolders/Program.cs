using Ookii.CommandLine;
using SpecialFolders.Configuration;

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
            .ToDictionary(
                sf => sf,
                sf => Environment.GetFolderPath(sf)
            );

        var specialFoldersList = specialFolders
                .Where(sf => string.IsNullOrWhiteSpace(arguments.FolderName?.ToString()) || sf.Key == arguments.FolderName)
                .Where(sf => !arguments.HideEmptyLocation || !string.IsNullOrWhiteSpace(sf.Value))
                .ToList();

        var maxNameWith = arguments.AlignNames
            ? specialFoldersList.Max(sf => sf.ToString().Length)
            : 0;

        var locationsText = string.Empty;

        foreach (var sf in specialFoldersList)
        {
            if (!string.IsNullOrWhiteSpace(locationsText))
                locationsText += Environment.NewLine;
            locationsText += Environment.GetFolderPath(sf.Key);

            Console.Out.WriteLine("{0} : {1}",
                arguments.AlignNames
                    ? sf.ToString().PadRight(maxNameWith)
                    : sf,
                sf.Value
            );
        }
    }
}
