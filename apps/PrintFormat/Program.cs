using Ookii.CommandLine;
using PrintFormat.Configuration;

namespace PrintFormat;

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
        var args = arguments.FormatArguments
            .Cast<object>()
            .ToArray();
        var text = string.Format(arguments.Format ?? string.Empty, args);

        await Console.Out.WriteLineAsync(text);
    }
}
