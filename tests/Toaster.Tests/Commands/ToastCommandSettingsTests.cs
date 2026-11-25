using Toaster.Commands;
using Toaster.Configuration;

namespace Toaster.Tests.Commands;

public class ToastCommandSettingsTests
{
    private static ToastCommand.Settings Valid() => new() { Text = "Hello" };

    [Fact]
    public void CustomValidate_passes_for_defaults()
    {
        Should.NotThrow(() => Valid().CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_text_is_empty()
    {
        var settings = new ToastCommand.Settings { Text = string.Empty };

        Should.Throw<ArgumentNullException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void CustomValidate_throws_when_timeout_seconds_is_negative(int timeoutSeconds)
    {
        var settings = Valid();
        settings.TimeoutSeconds = timeoutSeconds;

        Should.Throw<ArgumentOutOfRangeException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void CustomValidate_throws_when_transition_time_is_negative(int transitionTime)
    {
        var settings = Valid();
        settings.TransitionTime = transitionTime;

        Should.Throw<ArgumentOutOfRangeException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1)]
    public void CustomValidate_throws_when_horizontal_indent_is_negative(double horizontalIndent)
    {
        var settings = Valid();
        settings.HorizontalIndent = horizontalIndent;

        Should.Throw<ArgumentOutOfRangeException>(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_background_colour_is_invalid()
    {
        var settings = Valid();
        settings.BackgroundColour = "NotAColour";

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("Red:Green:Blue")]
    [InlineData("Red:NotAColour")]
    public void CustomValidate_throws_when_background_colour_gradient_pair_is_invalid(string backgroundColour)
    {
        var settings = Valid();
        settings.BackgroundColour = backgroundColour;

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_fill_style_is_not_solid_without_a_colour()
    {
        var settings = Valid();
        settings.BackgroundFillStyle = GradientFillStyleType.Vertical;

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_fill_style_is_not_solid_with_only_one_colour()
    {
        var settings = Valid();
        settings.BackgroundFillStyle = GradientFillStyleType.Horizontal;
        settings.BackgroundColour = "Red";

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(GradientFillStyleType.Vertical)]
    [InlineData(GradientFillStyleType.Horizontal)]
    [InlineData(GradientFillStyleType.DiagonalFromTopLeft)]
    [InlineData(GradientFillStyleType.DiagonalFromTopRight)]
    public void CustomValidate_accepts_fill_style_not_solid_with_two_colours(GradientFillStyleType fillStyle)
    {
        var settings = Valid();
        settings.BackgroundFillStyle = fillStyle;
        settings.BackgroundColour = "Red:Blue";

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("notanumber")]
    [InlineData("0")]
    [InlineData("-1")]
    public void CustomValidate_throws_when_window_width_is_invalid(string windowWidth)
    {
        var settings = Valid();
        settings.WindowWidth = windowWidth;

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(Constants.Auto)]
    [InlineData("")]
    [InlineData("auto")]
    [InlineData("360")]
    [InlineData("500.5")]
    public void CustomValidate_accepts_valid_window_width(string windowWidth)
    {
        var settings = Valid();
        settings.WindowWidth = windowWidth;

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_foreground_colour_is_invalid()
    {
        var settings = Valid();
        settings.ForegroundColour = "NotAColour";

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(Constants.Auto)]
    [InlineData("")]
    [InlineData("Red")]
    [InlineData("#FF0000")]
    public void CustomValidate_accepts_valid_foreground_colour(string foregroundColour)
    {
        var settings = Valid();
        settings.ForegroundColour = foregroundColour;

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("NotASound")]
    [InlineData("C:/not/a/real/path.wav")]
    [InlineData("C:/not/even/a/wav.mp3")]
    public void CustomValidate_throws_when_sound_is_invalid(string sound)
    {
        var settings = Valid();
        settings.Sound = sound;

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Asterisk")]
    [InlineData("beep")]
    public void CustomValidate_accepts_valid_sound(string sound)
    {
        var settings = Valid();
        settings.Sound = sound;

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_text_font_name_is_invalid()
    {
        var settings = Valid();
        settings.TextFontName = "NotARealFontXYZ";

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Arial")]
    [InlineData("Segoe UI")]
    public void CustomValidate_accepts_valid_text_font_name(string fontName)
    {
        var settings = Valid();
        settings.TextFontName = fontName;

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CustomValidate_throws_when_text_font_size_is_not_positive(double fontSize)
    {
        var settings = Valid();
        settings.TextFontSize = fontSize;

        Should.Throw<ArgumentOutOfRangeException>(() => settings.CustomValidate());
    }

    [Fact]
    public void CustomValidate_throws_when_title_font_name_is_invalid()
    {
        var settings = Valid();
        settings.TitleFontName = "NotARealFontXYZ";

        Should.Throw<ArgumentException>(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Arial")]
    [InlineData("Segoe UI")]
    public void CustomValidate_accepts_valid_title_font_name(string fontName)
    {
        var settings = Valid();
        settings.TitleFontName = fontName;

        Should.NotThrow(() => settings.CustomValidate());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CustomValidate_throws_when_title_font_size_is_not_positive(double fontSize)
    {
        var settings = Valid();
        settings.TitleFontSize = fontSize;

        Should.Throw<ArgumentOutOfRangeException>(() => settings.CustomValidate());
    }

    [Fact]
    public void ToToastRequest_maps_every_field()
    {
        var settings = new ToastCommand.Settings
        {
            Text = "Hello",
            Title = "Title",
            TimeoutSeconds = 7,
            Location = ToastLocationType.TopLeft,
            Monitor = ToastMonitorType.All,
            BackgroundColour = "Red",
            ForegroundColour = "Blue",
            TextAlignment = TextAlignmentType.Centre,
            TitleAlignment = TextAlignmentType.Right,
            TransitionTime = 1234,
            HorizontalIndent = 0.5,
            BackgroundFillStyle = GradientFillStyleType.DiagonalFromTopLeft,
            WindowWidth = "500",
            Sound = "Asterisk",
            TextFontName = "Arial",
            TextFontSize = 16,
            TitleFontName = "Segoe UI",
            TitleFontSize = 18,
            TextFontStyle = FontStyleType.Bold,
            TitleFontStyle = FontStyleType.Italic,
        };

        var request = settings.ToToastRequest();

        request.Text.ShouldBe("Hello");
        request.Title.ShouldBe("Title");
        request.TimeoutSeconds.ShouldBe(7);
        request.Location.ShouldBe(ToastLocationType.TopLeft);
        request.Monitor.ShouldBe(ToastMonitorType.All);
        request.BackgroundColour.ShouldBe("Red");
        request.ForegroundColour.ShouldBe("Blue");
        request.TextAlignment.ShouldBe(TextAlignmentType.Centre);
        request.TitleAlignment.ShouldBe(TextAlignmentType.Right);
        request.TransitionTimeMilliseconds.ShouldBe(1234);
        request.HorizontalIndent.ShouldBe(0.5);
        request.BackgroundFillStyle.ShouldBe(GradientFillStyleType.DiagonalFromTopLeft);
        request.WindowWidth.ShouldBe("500");
        request.Sound.ShouldBe("Asterisk");
        request.TextFontName.ShouldBe("Arial");
        request.TextFontSize.ShouldBe(16);
        request.TitleFontName.ShouldBe("Segoe UI");
        request.TitleFontSize.ShouldBe(18);
        request.TextFontStyle.ShouldBe(FontStyleType.Bold);
        request.TitleFontStyle.ShouldBe(FontStyleType.Italic);
    }
}
