using ConsoleApplications.Common.CommandLine;
using FigLetText.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace FigLetText;

internal class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var app = new CommandApp<FigletCommand>();
            app.Configure(config =>
            {
                FigletCommand.GetExamples().ToList().ForEach(e => config.AddExample(e));

                CommonCommandApp.Configure(config);
            });

            return app.Run(args);
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
