using System.Windows;
using System.Windows.Threading;
using Toaster.Configuration;

namespace Toaster;

/// <summary>
/// Owns every visible toast. Toasts are grouped into one stack per (monitor, corner); a single request
/// can span several stacks, in which case all its windows expire or are clicked away together. UI thread only.
/// </summary>
internal sealed class ToastHost
{
    private const double Margin = 16;
    private const double Gap = 10;

    private static readonly ToastLocationType[] Corners =
    {
        ToastLocationType.BottomRight, ToastLocationType.BottomLeft, ToastLocationType.TopRight, ToastLocationType.TopLeft,
    };

    private sealed class Stack
    {
        public Stack(Rect workArea, ToastLocationType corner)
        {
            WorkArea = workArea;
            Corner = corner;
        }

        public Rect WorkArea { get; }
        public ToastLocationType Corner { get; }
        public List<ToastWindow> Toasts { get; } = new(); // index 0 = nearest the screen edge
        public bool AtBottom => Corner is ToastLocationType.BottomRight or ToastLocationType.BottomLeft;
        public bool AtRight => Corner is ToastLocationType.BottomRight or ToastLocationType.TopRight;
    }

    private sealed class Group
    {
        public List<(ToastWindow Window, Stack Stack)> Members { get; } = new();
        public DispatcherTimer? Timer { get; set; }
        public bool Dismissed { get; set; }
    }

    private readonly Dictionary<string, Stack> _stacks = new();
    private readonly DispatcherTimer _idleTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    private int _liveWindows;

    public bool IsClosing { get; private set; }

    /// <summary>Raised once no toasts remain (after a short grace period). The host should then exit.</summary>
    public event Action? Idle;

    public ToastHost()
    {
        _idleTimer.Tick += (_, _) =>
        {
            _idleTimer.Stop();
            if (_liveWindows > 0)
                return;

            IsClosing = true;
            Idle?.Invoke();
        };
    }

    /// <summary>Shows a toast. False if the host is already shutting down and the caller must retry.</summary>
    public bool TryShow(ToastRequest request)
    {
        if (IsClosing)
            return false;

        _idleTimer.Stop();

        var group = new Group();
        var touched = new List<Stack>();

        foreach (var (monitorIndex, monitor) in SelectMonitors(request.Monitor))
        {
            foreach (var corner in SelectCorners(request.Location))
            {
                var key = $"{monitorIndex}:{corner}";
                if (!_stacks.TryGetValue(key, out var stack))
                    _stacks[key] = stack = new Stack(monitor.WorkArea, corner);

                var toast = new ToastWindow(request);
                toast.Clicked += _ => Dismiss(group);
                // Horizontal indent shifts the toast inward from its edge, in units of the toast's own width
                var indent = toast.Width * request.HorizontalIndent;
                toast.Left = stack.AtRight
                    ? stack.WorkArea.Right - Margin - toast.Width - indent
                    : stack.WorkArea.Left + Margin + indent;
                // Start just off the screen edge it will slide in from
                toast.Top = stack.AtBottom ? stack.WorkArea.Bottom : stack.WorkArea.Top - toast.Height;
                toast.WorkArea = stack.WorkArea; // clip to the monitor minus the taskbar while sliding

                stack.Toasts.Add(toast);
                group.Members.Add((toast, stack));
                touched.Add(stack);
                _liveWindows++;

                toast.Show();
                toast.FadeIn();
            }
        }

        if (request.TimeoutSeconds > 0)
        {
            group.Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(request.TimeoutSeconds) };
            group.Timer.Tick += (_, _) => Dismiss(group);
            group.Timer.Start();
        }

        foreach (var stack in touched.Distinct())
            Reflow(stack);

        return true;
    }

    private void Dismiss(Group group)
    {
        if (group.Dismissed)
            return;

        group.Dismissed = true;
        group.Timer?.Stop();

        foreach (var (toast, stack) in group.Members)
            stack.Toasts.Remove(toast);

        // Whatever is left in each stack closes the gap while the dismissed toasts slide out
        foreach (var stack in group.Members.Select(m => m.Stack).Distinct())
            Reflow(stack);

        foreach (var (toast, stack) in group.Members)
        {
            var exitTop = stack.AtBottom ? stack.WorkArea.Bottom : stack.WorkArea.Top - toast.Height;
            toast.SlideOutTo(exitTop, () =>
            {
                toast.Close();
                if (--_liveWindows == 0)
                    _idleTimer.Start();
            });
        }
    }

    private static void Reflow(Stack stack)
    {
        if (stack.AtBottom)
        {
            var edge = stack.WorkArea.Bottom - Margin;
            foreach (var toast in stack.Toasts)
            {
                var top = edge - toast.Height;
                toast.MoveTo(top);
                edge = top - Gap;
            }
        }
        else
        {
            var edge = stack.WorkArea.Top + Margin;
            foreach (var toast in stack.Toasts)
            {
                toast.MoveTo(edge);
                edge += toast.Height + Gap;
            }
        }
    }

    private static IEnumerable<(int Index, Monitors.MonitorInfo Monitor)> SelectMonitors(ToastMonitorType type)
    {
        var all = Monitors.GetAll();
        var indexed = all.Select((m, i) => (i, m));

        return type == ToastMonitorType.All
            ? indexed
            : indexed.Where(x => x.m.IsPrimary).DefaultIfEmpty((0, all[0]));
    }

    private static IEnumerable<ToastLocationType> SelectCorners(ToastLocationType type) =>
        type == ToastLocationType.All ? Corners : new[] { type };
}
