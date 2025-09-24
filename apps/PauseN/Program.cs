using System.CommandLine.Help;
using ConsoleApplications.Common.CommandLine;
using DotMake.CommandLine;
using PauseN.Configuration;

namespace PauseN;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var parser = Cli.GetParser<Arguments>();
            var result = await parser.RunAsync(args);

            //await Cli.RunAsync<Arguments>();

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }
}
