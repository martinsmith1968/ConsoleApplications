using System.IO;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace Toaster;

/// <summary>
/// Cross-instance coordination. The first instance to take the named mutex is the host and owns
/// all toast windows; other instances hand their request to it over a named pipe and exit.
/// </summary>
internal static class InstanceCoordinator
{
    private static readonly string Suffix = $"Toaster.{Process.GetCurrentProcess().SessionId}";
    private static string PipeName => $"{Suffix}.Pipe";

    public static Mutex? TryBecomeHost()
    {
        var mutex = new Mutex(false, $@"Local\{Suffix}.Host");
        try
        {
            if (mutex.WaitOne(0))
                return mutex;
        }
        catch (AbandonedMutexException)
        {
            return mutex; // previous host died; we own it now
        }

        mutex.Dispose();
        return null;
    }

    /// <summary>Hands the request to the host. False if there is no host or it declined (shutting down).</summary>
    public static bool TrySend(ToastRequest request)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.CurrentUserOnly);
            client.Connect(500);

            using var writer = new StreamWriter(client, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
            using var reader = new StreamReader(client, Encoding.UTF8, leaveOpen: true);

            writer.WriteLine(JsonSerializer.Serialize(request));
            return reader.ReadLine() == "OK";
        }
        catch (Exception e) when (e is TimeoutException or IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    /// <summary>Host side: accepts requests until cancelled. The handler returns false to decline.</summary>
    public static async Task ServeAsync(Func<ToastRequest, Task<bool>> handler, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);

                await server.WaitForConnectionAsync(cancellationToken);

                using var reader = new StreamReader(server, Encoding.UTF8, leaveOpen: true);
                await using var writer = new StreamWriter(server, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };

                var line = await reader.ReadLineAsync(cancellationToken);
                var request = line is null ? null : JsonSerializer.Deserialize<ToastRequest>(line);

                var accepted = request is not null && await handler(request);
                await writer.WriteLineAsync(accepted ? "OK" : "NO");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                // A misbehaving client must not take the host down; carry on listening
            }
        }
    }
}
