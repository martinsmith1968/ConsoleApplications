using Ookii.CommandLine;
using PauseN.Configuration;

namespace PauseN;

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
        await Console.Out.WriteAsync(arguments.DisplayText);

        var timeoutDate = DateTime.UtcNow.AddSeconds(arguments.TimeoutSeconds);

        while (DateTime.UtcNow < timeoutDate)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        await Console.Out.WriteLineAsync();
    }
}
