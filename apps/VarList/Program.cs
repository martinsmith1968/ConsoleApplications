using ConsoleApplications.Common.CommandLine;
using Spectre.Console;
using Spectre.Console.Cli;
using VarList.Commands;

namespace VarList;

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            var app = new CommandApp<VarListCommand>();
            app.Configure(config =>
            {
                CommonCommandApp.Configure(config);
            });


            app.Run(args);
        }
        catch (CommandRuntimeException ex)
        {
            AnsiConsole.MarkupLine($"[red]ERROR:[/] {ex.Message}");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
    }
}
