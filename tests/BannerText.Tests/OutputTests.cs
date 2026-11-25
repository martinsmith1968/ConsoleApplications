using Snapshooter.Xunit3;

namespace BannerText.Tests;

public class OutputTests : BaseOutputTest
{
    [Fact]
    public async Task Default_hello_world()
    {
        var args = new[] { "Hello World" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Min_total_length_expansion()
    {
        var args = new[] { "Hello World", "--minl", "80" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Center_alignment_with_min_total_length()
    {
        var args = new[] { "Hello World", "--minl", "80", "--ta", "Center" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Right_alignment_with_min_total_length()
    {
        var args = new[] { "Hello World", "--minl", "80", "--ta", "Right" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Custom_header_and_footer_characters()
    {
        var args = new[] { "Test", "--hlc", "#", "--flc", "#" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Multiple_text_lines()
    {
        var args = new[] { "Hello", "World" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }

    [Fact]
    public async Task Zero_header_and_footer_lines()
    {
        var args = new[] { "Hi", "--hln", "0", "--fln", "0" };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        Snapshot.Match(OutputText.ToString());
    }
}
