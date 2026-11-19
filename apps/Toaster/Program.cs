using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;
using Toaster.Commands;

namespace Toaster;

internal class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // WinExe has no console of its own; borrow the parent's so errors/usage are still visible
        Win32.AttachConsole(-1);

        try
        {
            var app = new CommandApp<ToastCommand>();
            app.Configure(config =>
            {
                ToastCommand.GetExamples().ToList().ForEach(e => config.AddExample(e));

                CommonCommandApp.Configure(config);
            });

            // Must stay synchronous: an awaited RunAsync would resume on a thread-pool
            // thread (no SynchronizationContext in a console app), losing STA-ness
            // before the WPF Window gets created.
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
