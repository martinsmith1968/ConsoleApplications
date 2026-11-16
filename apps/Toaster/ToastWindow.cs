using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Toaster;

/// <summary>A single borderless toast card.</summary>
internal sealed class ToastWindow : Window
{
    public const double ToastWidth = 360;
    private static readonly TimeSpan AnimationTime = TimeSpan.FromMilliseconds(300);

    public DispatcherTimer? ExpiryTimer { get; set; }
    public event Action<ToastWindow>? Clicked;

    public ToastWindow(ToastRequest request)
    {
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
        var titleBrush = light ? Brushes.Black : Brushes.White;
        var textBrush = new SolidColorBrush(light ? Color.FromRgb(0x22, 0x22, 0x22) : Color.FromRgb(0xDD, 0xDD, 0xDD));

        var stack = new StackPanel();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            stack.Children.Add(new TextBlock
            {
                Text = request.Title,
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

        card.Measure(new Size(ToastWidth, double.PositiveInfinity));
        Height = Math.Ceiling(card.DesiredSize.Height);

        card.MouseLeftButtonUp += (_, _) => Clicked?.Invoke(this);
        SourceInitialized += (_, _) => HideFromAltTab();
    }

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
