using System.Security.AccessControl;
using Ookii.CommandLine;
using DisplaySettings.Configuration;

namespace DisplaySettings;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args, Arguments.Options)
                            ?? throw new Exception("Unable to Parse Command Line");

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
        switch (arguments.Command)
        {
            case CommandType.View:
                await ViewDisplaySettings(arguments);
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(arguments.Command));
        }
    }

    private static async Task ViewDisplaySettings(Arguments arguments)
    {

    }
}
