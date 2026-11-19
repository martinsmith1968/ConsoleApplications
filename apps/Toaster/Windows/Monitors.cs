using System.Runtime.InteropServices;
using System.Windows;

namespace Toaster.Windows;

/// <summary>Enumerates monitor work areas, in the device-independent units WPF windows are positioned in.</summary>
internal static class Monitors
{
    public record MonitorInfo(Rect WorkArea, bool IsPrimary);

    public static IReadOnlyList<MonitorInfo> GetAll()
    {
        var scale = GetSystemScale();
        var monitors = new List<MonitorInfo>();

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr _, ref RECT _, IntPtr _) =>
        {
            var info = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
            if (GetMonitorInfo(hMonitor, ref info))
            {
                var work = info.rcWork;
                monitors.Add(new MonitorInfo(
                    new Rect(work.Left / scale, work.Top / scale, (work.Right - work.Left) / scale, (work.Bottom - work.Top) / scale),
                    (info.dwFlags & MONITORINFOF_PRIMARY) != 0));
            }

            return true;
        }, IntPtr.Zero);

        if (monitors.Count == 0)
            monitors.Add(new MonitorInfo(SystemParameters.WorkArea, true));

        return monitors;
    }

    private static double GetSystemScale()
    {
        var dc = GetDC(IntPtr.Zero);
        try
        {
            return GetDeviceCaps(dc, LOGPIXELSX) / 96.0;
        }
        finally
        {
            ReleaseDC(IntPtr.Zero, dc);
        }
    }

    private const int LOGPIXELSX = 88;
    private const int MONITORINFOF_PRIMARY = 1;

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern int GetDeviceCaps(IntPtr hdc, int index);
}
