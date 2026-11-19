using Ookii.CommandLine;
using Toaster.Configuration;

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
            var arguments = CommandLineParser.Parse<Arguments>(args);
            if (arguments is null)
                return 1;

            arguments.Validate();
            var request = arguments.ToToastRequest();

            return ToastRequestHandler.Handle(request);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            return 1;
        }
    }
}
