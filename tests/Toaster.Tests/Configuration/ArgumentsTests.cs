using Toaster.Configuration;

namespace Toaster.Tests.Configuration;

public class ArgumentsTests
{
    private static Arguments Valid() => new() { Text = "Hello" };

    [Fact]
    public void Validate_passes_for_defaults()
    {
        Should.NotThrow(() => Valid().Validate());
    }

    [Fact]
    public void Validate_throws_when_text_is_empty()
    {
        var arguments = new Arguments { Text = string.Empty };

        Should.Throw<ArgumentNullException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Validate_throws_when_timeout_seconds_is_negative(int timeoutSeconds)
    {
        var arguments = Valid();
        arguments.TimeoutSeconds = timeoutSeconds;

        Should.Throw<ArgumentOutOfRangeException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Validate_throws_when_transition_time_is_negative(int transitionTimeMilliseconds)
    {
        var arguments = Valid();
        arguments.TransitionTimeMilliseconds = transitionTimeMilliseconds;

        Should.Throw<ArgumentOutOfRangeException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1)]
    public void Validate_throws_when_horizontal_indent_is_negative(double horizontalIndent)
    {
        var arguments = Valid();
        arguments.HorizontalIndent = horizontalIndent;

        Should.Throw<ArgumentOutOfRangeException>(() => arguments.Validate());
    }

    [Fact]
    public void Validate_throws_when_background_colour_is_invalid()
    {
        var arguments = Valid();
        arguments.BackgroundColour = "NotAColour";

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData("Red:Green:Blue")]
    [InlineData("Red:NotAColour")]
    public void Validate_throws_when_background_colour_gradient_pair_is_invalid(string backgroundColour)
    {
        var arguments = Valid();
        arguments.BackgroundColour = backgroundColour;

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Fact]
    public void Validate_throws_when_fill_style_is_not_solid_without_a_colour()
    {
        var arguments = Valid();
        arguments.BackgroundFillStyle = GradientFillStyleType.Vertical;

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Fact]
    public void Validate_throws_when_fill_style_is_not_solid_with_only_one_colour()
    {
        var arguments = Valid();
        arguments.BackgroundFillStyle = GradientFillStyleType.Horizontal;
        arguments.BackgroundColour = "Red";

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(GradientFillStyleType.Vertical)]
    [InlineData(GradientFillStyleType.Horizontal)]
    [InlineData(GradientFillStyleType.DiagonalFromTopLeft)]
    [InlineData(GradientFillStyleType.DiagonalFromTopRight)]
    public void Validate_accepts_fill_style_not_solid_with_two_colours(GradientFillStyleType fillStyle)
    {
        var arguments = Valid();
        arguments.BackgroundFillStyle = fillStyle;
        arguments.BackgroundColour = "Red:Blue";

        Should.NotThrow(() => arguments.Validate());
    }

    [Theory]
    [InlineData("notanumber")]
    [InlineData("0")]
    [InlineData("-1")]
    public void Validate_throws_when_window_width_is_invalid(string windowWidth)
    {
        var arguments = Valid();
        arguments.WindowWidth = windowWidth;

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(Constants.Auto)]
    [InlineData("")]
    [InlineData("auto")]
    [InlineData("360")]
    [InlineData("500.5")]
    public void Validate_accepts_valid_window_width(string windowWidth)
    {
        var arguments = Valid();
        arguments.WindowWidth = windowWidth;

        Should.NotThrow(() => arguments.Validate());
    }

    [Fact]
    public void Validate_throws_when_foreground_colour_is_invalid()
    {
        var arguments = Valid();
        arguments.ForegroundColour = "NotAColour";

        Should.Throw<ArgumentException>(() => arguments.Validate());
    }

    [Theory]
    [InlineData(Constants.Auto)]
    [InlineData("")]
    [InlineData("Red")]
    [InlineData("#FF0000")]
    public void Validate_accepts_valid_foreground_colour(string foregroundColour)
    {
        var arguments = Valid();
        arguments.ForegroundColour = foregroundColour;

        Should.NotThrow(() => arguments.Validate());
    }

    [Fact]
    public void ToToastRequest_maps_every_field()
    {
        var arguments = new Arguments
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
            TransitionTimeMilliseconds = 1234,
            HorizontalIndent = 0.5,
            BackgroundFillStyle = GradientFillStyleType.DiagonalFromTopLeft,
            WindowWidth = "500",
        };

        var request = arguments.ToToastRequest();

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
    }
}
