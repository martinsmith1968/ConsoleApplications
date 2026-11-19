using System.Runtime.InteropServices;

namespace Toaster;

internal class Win32
{

    [DllImport("kernel32.dll")]
    public static extern bool AttachConsole(int dwProcessId);
}
