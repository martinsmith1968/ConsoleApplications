using System.Windows;

namespace Toaster;

/// <summary>Shows a toast for an already-built <see cref="ToastRequest"/>, becoming the host or handing off to it.</summary>
internal static class ToastRequestHandler
{
    /// <summary>Either becomes the host, or hands off to it. Retry covers a host that is starting up or shutting down.</summary>
    public static int Handle(ToastRequest request)
    {
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
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
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
}
