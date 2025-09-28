using Ookii.CommandLine;
using SpecialFolders.Configuration;
using SpecialFolders.Configuration.Types;
using SpecialFolders.Writers;

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

        var writer = WriterFactory.CreateWriter(arguments.Writer);

        await writer.WriteOutputAsync(specialFoldersList, arguments);
    }
}
