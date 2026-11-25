using ConsoleApplications.Common.CommandLine;
using GUIDGenerator.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GUIDGenerator;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp<GUIDGeneratorCommand>();
            app.Configure(config =>
            {
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
