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
    internal const double DefaultWidth = 360;
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
        Opacity = 0;

        var backgroundBrush = CreateBackgroundBrush(request.BackgroundColour, request.BackgroundFillStyle, out var background);
        var light = ColourParser.IsLight(background);

        // Auto: pick for contrast against the background. Otherwise use the requested colour for title and message.
        Brush titleBrush = light ? Brushes.Black : Brushes.White;
        Brush textBrush = new SolidColorBrush(light ? Color.FromRgb(0x22, 0x22, 0x22) : Color.FromRgb(0xDD, 0xDD, 0xDD));
        if (!ColourParser.IsAuto(request.ForegroundColour) && ColourParser.TryParse(request.ForegroundColour, out var foreground))
            titleBrush = textBrush = new SolidColorBrush(foreground);

        var stack = new StackPanel();

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            var (titleWeight, titleStyle) = ToWpf(request.TitleFontStyle, FontWeights.SemiBold);
            var titleBlock = new TextBlock
            {
                Text = request.Title,
                TextAlignment = ToWpf(request.TitleAlignment),
                FontWeight = titleWeight,
                FontStyle = titleStyle,
                FontSize = request.TitleFontSize,
                Foreground = titleBrush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 4),
            };
            if (!string.IsNullOrWhiteSpace(request.TitleFontName))
                titleBlock.FontFamily = new FontFamily(request.TitleFontName);

            stack.Children.Add(titleBlock);
        }

        var (textWeight, textStyle) = ToWpf(request.TextFontStyle, FontWeights.Normal);
        var textBlock = new TextBlock
        {
            Text = request.Text,
            TextAlignment = ToWpf(request.TextAlignment),
            FontWeight = textWeight,
            FontStyle = textStyle,
            FontSize = request.TextFontSize,
            Foreground = textBrush,
            TextWrapping = TextWrapping.Wrap,
        };
        if (!string.IsNullOrWhiteSpace(request.TextFontName))
            textBlock.FontFamily = new FontFamily(request.TextFontName);

        stack.Children.Add(textBlock);

        var card = new Border
        {
            Background = backgroundBrush,
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

        if (Constants.IsAuto(request.WindowWidth))
        {
            // Unconstrained measure: the card sizes to its content (title/text) plus its own Padding/BorderThickness.
            card.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Width = Math.Ceiling(card.DesiredSize.Width);
        }
        else
        {
            Width = double.TryParse(request.WindowWidth, out var fixedWidth) && fixedWidth > 0 ? fixedWidth : DefaultWidth;
            card.Measure(new Size(Width, double.PositiveInfinity));
        }

        Height = Math.Ceiling(card.DesiredSize.Height);

        card.MouseLeftButtonUp += (_, _) => Clicked?.Invoke(this);
        SourceInitialized += (_, _) => HideFromAltTab();
    }

    /// <summary>
    /// A solid brush for a single colour (or when <paramref name="fillStyle"/> is Solid), otherwise a gradient
    /// from the first colour to the second in the direction <paramref name="fillStyle"/> names.
    /// <paramref name="representative"/> is the colour to use for auto-contrast decisions: the single colour,
    /// or the average of the two gradient colours.
    /// </summary>
    private static Brush CreateBackgroundBrush(string? backgroundColour, GradientFillStyleType fillStyle, out Color representative)
    {
        if (!ColourParser.TryParsePair(backgroundColour, out var first, out var second))
        {
            representative = Color.FromRgb(0x2B, 0x2B, 0x2B);
            return new SolidColorBrush(representative);
        }

        if (fillStyle == GradientFillStyleType.Solid || second is null)
        {
            representative = first;
            return new SolidColorBrush(first);
        }

        representative = AverageColour(first, second.Value);

        var (start, end) = fillStyle switch
        {
            GradientFillStyleType.Horizontal => (new Point(0, 0.5), new Point(1, 0.5)),
            GradientFillStyleType.DiagonalFromTopLeft => (new Point(0, 0), new Point(1, 1)),
            GradientFillStyleType.DiagonalFromTopRight => (new Point(1, 0), new Point(0, 1)),
            _ => (new Point(0.5, 0), new Point(0.5, 1)), // Vertical
        };

        return new LinearGradientBrush(first, second.Value, start, end);
    }

    private static Color AverageColour(Color a, Color b) => Color.FromArgb(
        (byte)((a.A + b.A) / 2),
        (byte)((a.R + b.R) / 2),
        (byte)((a.G + b.G) / 2),
        (byte)((a.B + b.B) / 2));

    private static TextAlignment ToWpf(TextAlignmentType alignment) => alignment switch
    {
        TextAlignmentType.Right => TextAlignment.Right,
        TextAlignmentType.Centre => TextAlignment.Center,
        _ => TextAlignment.Left,
    };

    /// <summary>
    /// Maps a <see cref="FontStyleType"/> to a WPF (weight, style) pair. <see cref="FontStyleType.Default"/>
    /// keeps the element's own default weight (<paramref name="defaultWeight"/>) and never italicises; every
    /// other value fully specifies both weight and style.
    /// </summary>
    private static (FontWeight Weight, FontStyle Style) ToWpf(FontStyleType fontStyle, FontWeight defaultWeight) => fontStyle switch
    {
        FontStyleType.Regular => (FontWeights.Normal, FontStyles.Normal),
        FontStyleType.Bold => (FontWeights.Bold, FontStyles.Normal),
        FontStyleType.Italic => (FontWeights.Normal, FontStyles.Italic),
        FontStyleType.BoldItalic => (FontWeights.Bold, FontStyles.Italic),
        _ => (defaultWeight, FontStyles.Normal),
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
