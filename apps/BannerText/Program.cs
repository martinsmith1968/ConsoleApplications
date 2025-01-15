using BannerText.Configuration;
using Ookii.CommandLine;

namespace BannerText;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args, Arguments.Options)
                            ?? throw new Exception("Unable to Parse Command Line");
            arguments.Validate();

            await ProcessAsync(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }

    private static async Task ProcessAsync(Arguments arguments)
    {
        // TODO:
        foreach (var text in arguments.Text)
        {
            await Console.Out.WriteLineAsync(text);
        }
    }
}
