using ConsoleApplications.Common.CommandLine;
using PauseN.Configuration;
using Spectre.Console.Cli;

namespace PauseN;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp<PauseNCommand>();
            app.Configure(config =>
            {
                CustomCommandAppConfiguration.Configure(config);

                config.AddExample("");
                config.AddExample("5");
                config.AddExample("10", "-t", $"\"Pausing for {PauseNCommand.Settings.PlaceHolder_TimeoutSeconds} seconds...\"");
            });

            return await app.RunAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }
    }
}
