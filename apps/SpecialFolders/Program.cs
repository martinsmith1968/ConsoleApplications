using ConsoleApplications.Common.CommandLine;
using SpecialFolders.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SpecialFolders;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp<SpecialFoldersCommand>();
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
