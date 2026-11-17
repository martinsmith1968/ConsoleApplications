using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Toaster.Configuration;

namespace Toaster;

/// <summary>A single borderless toast card.</summary>
internal sealed class ToastWindow : Window
{
    public const double ToastWidth = 360;
    private readonly TimeSpan AnimationTime;

    private readonly Border _card;
    private Rect _workArea = Rect.Empty;

    public DispatcherTimer? ExpiryTimer { get; set; }

    /// <summary>
    /// The screen area (monitor minus taskbar) the toast may draw in. Toasts slide in from / out to just beyond
    /// this area, so anything outside it is clipped rather than drawn over the taskbar.
    /// </summary>
    public Rect WorkArea
    {
        get => _workArea;
        set
        {
            _workArea = value;
            UpdateClip();
        }
    }

    private double _clippedForTop = double.NaN;

    private void OnRendering(object? sender, EventArgs e)
    {
        if (Top != _clippedForTop)
            UpdateClip();
    }

    private void UpdateClip()
    {
        if (_workArea.IsEmpty)
            return;

        _clippedForTop = Top;

        var visible = Rect.Intersect(new Rect(Left, Top, Width, Height), _workArea);

        // Local (window) coordinates; nothing visible = empty clip
        _card.Clip = new RectangleGeometry(visible.IsEmpty
            ? Rect.Empty
            : new Rect(visible.X - Left, visible.Y - Top, visible.Width, visible.Height));
    }
    public event Action<ToastWindow>? Clicked;

    public ToastWindow(ToastRequest request)
    {
        AnimationTime = TimeSpan.FromMilliseconds(Math.Max(0, request.TransitionTimeMilliseconds));

        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ShowInTaskbar = false;
        ShowActivated = false;
        Topmost = true;
        Width = ToastWidth;
        Opacity = 0;

        var background = ColourParser.TryParse(request.BackgroundColour, out var parsed)
            ? parsed
            : Color.FromRgb(0x2B, 0x2B, 0x2B);
        var light = ColourParser.IsLight(background);

        // Auto: pick for contrast against the background. Otherwise use the requested colour for title and message.
        Brush titleBrush = light ? Brushes.Black : Brushes.White;
        Brush textBrush = new SolidColorBrush(light ? Color.FromRgb(0x22, 0x22, 0x22) : Color.FromRgb(0xDD, 0xDD, 0xDD));
        if (!ColourParser.IsAuto(request.ForegroundColour) && ColourParser.TryParse(request.ForegroundColour, out var foreground))
            titleBrush = textBrush = new SolidColorBrush(foreground);

        var stack = new StackPanel();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            stack.Children.Add(new TextBlock
            {
                Text = request.Title,
                TextAlignment = ToWpf(request.TitleAlignment),
                FontWeight = FontWeights.SemiBold,
                FontSize = 14,
                Foreground = titleBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4),
            });
        }

        stack.Children.Add(new TextBlock
        {
            Text = request.Text,
            TextAlignment = ToWpf(request.TextAlignment),
            FontSize = 13,
            Foreground = textBrush,
            TextWrapping = TextWrapping.Wrap,
        });

        var card = new Border
        {
            Background = new SolidColorBrush(background),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(16, 12, 16, 14),
            Cursor = Cursors.Hand,
            Child = stack,
        };
        Content = card;
        _card = card;
        // LocationChanged isn't reliably raised while Top is being animated, so refresh the clip every frame
        LocationChanged += (_, _) => UpdateClip();
        CompositionTarget.Rendering += OnRendering;
        Closed += (_, _) => CompositionTarget.Rendering -= OnRendering;

        card.Measure(new Size(ToastWidth, double.PositiveInfinity));
        Height = Math.Ceiling(card.DesiredSize.Height);

        card.MouseLeftButtonUp += (_, _) => Clicked?.Invoke(this);
        SourceInitialized += (_, _) => HideFromAltTab();
    }

    private static TextAlignment ToWpf(TextAlignmentType alignment) => alignment switch
    {
        TextAlignmentType.Right => TextAlignment.Right,
        TextAlignmentType.Centre => TextAlignment.Center,
        _ => TextAlignment.Left,
    };

    public void FadeIn() =>
        BeginAnimation(OpacityProperty, new DoubleAnimation(1, AnimationTime));

    public void MoveTo(double top) =>
        BeginAnimation(TopProperty, new DoubleAnimation(top, AnimationTime)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        });

    /// <summary>Slides down (and fades) to <paramref name="top"/>, then calls <paramref name="completed"/>.</summary>
    public void SlideOutTo(double top, Action completed)
    {
        var slide = new DoubleAnimation(top, AnimationTime)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
        };
        slide.Completed += (_, _) => completed();

        BeginAnimation(OpacityProperty, new DoubleAnimation(0, AnimationTime));
        BeginAnimation(TopProperty, slide);
    }

    private void HideFromAltTab()
    {
        const int GWL_EXSTYLE = -20;
        const int WS_EX_TOOLWINDOW = 0x80;
        const int WS_EX_NOACTIVATE = 0x08000000;

        var handle = new WindowInteropHelper(this).Handle;
        var style = GetWindowLong(handle, GWL_EXSTYLE);
        SetWindowLong(handle, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
