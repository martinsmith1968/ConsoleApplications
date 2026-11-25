using BannerText.Commands;
using ConsoleApplications.Common.CommandLine;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BannerText;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp<BannerTextCommand>();
            app.Configure(config =>
            {
                BannerTextCommand.GetExamples().ToList().ForEach(e => config.AddExample(e));

                CommonCommandApp.Configure(config);
            });

            return await app.RunAsync(args);
        }
        catch (CommandRuntimeException ex)
        {
            AnsiConsole.MarkupLine($"[red]ERROR:[/] {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            return 2;
        }
    }
}
