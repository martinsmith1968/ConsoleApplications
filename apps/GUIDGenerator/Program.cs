using ConsoleApplications.Common.Configuration;
using ConsoleApplications.Common.Services;
using GUIDGenerator.Configuration;
using Ookii.CommandLine;

namespace GUIDGenerator;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args, Arguments.Options)
                ?? throw new Exception("Unable to Parse Command Line");

            arguments.Validate();

            Enumerable.Range(1, arguments.Count)
                .ToList()
                .ForEach(async x => await GenerateAsync(arguments));
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }

    private static async Task GenerateAsync(Arguments arguments)
    {
        var value = GUIDCreator.Create();

        var formatOptions = new TextGUIDFormatOptions()
        {
            GUIDFormat = arguments.GUIDFormat,
            Prefix = arguments.Prefix,
            Suffix = arguments.Suffix,
            CaseConversionType = arguments.CaseConversionType,
        };

        var displayValue = TextGUIDFormatter.FormatGUID(value, formatOptions);

        var writer = Console.Out;

        await writer.WriteAsync(displayValue);
    }
}
