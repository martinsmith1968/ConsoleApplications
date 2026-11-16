using System.Runtime.InteropServices;
using System.Windows;
using Ookii.CommandLine;
using Toaster.Configuration;

namespace Toaster;

internal class Program
{
    [STAThread]
    public static int Main(string[] args)
    {
        // WinExe has no console of its own; borrow the parent's so errors/usage are still visible
        AttachConsole(-1);

        ToastRequest request;
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args);
            if (arguments is null)
                return 1;

            arguments.Validate();
            request = new ToastRequest(arguments.Text, arguments.Title, arguments.TimeoutSeconds, arguments.Location, arguments.Monitor, arguments.BackgroundColour, arguments.ForegroundColour, arguments.TextAlignment, arguments.TitleAlignment);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            return 1;
        }

        // Either become the host, or hand off to it. Retry covers a host that is starting up or shutting down.
        var deadline = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < deadline)
        {
            using var hostMutex = InstanceCoordinator.TryBecomeHost();
            if (hostMutex is not null)
            {
                RunHost(request);
                hostMutex.ReleaseMutex();
                return 0;
            }

            if (InstanceCoordinator.TrySend(request))
                return 0;

            Thread.Sleep(100);
        }

        Console.Error.WriteLine("ERROR: Unable to display toast");
        return 2;
    }

    private static void RunHost(ToastRequest first)
    {
        var app = new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var host = new ToastHost();
        using var cts = new CancellationTokenSource();

        host.Idle += () =>
        {
            cts.Cancel();
            app.Shutdown();
        };

        app.Startup += (_, _) =>
        {
            host.TryShow(first);
            _ = InstanceCoordinator.ServeAsync(
                request => app.Dispatcher.InvokeAsync(() => host.TryShow(request)).Task,
                cts.Token);
        };

        app.Run();
    }

    [DllImport("kernel32.dll")]
    private static extern bool AttachConsole(int dwProcessId);
}
