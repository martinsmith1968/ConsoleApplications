using Shouldly;

namespace PrintFormat.Tests;

public class OutputTests : BaseOutputTest
{
    [Fact]
    public async Task Simple_text_with_single_argument()
    {
        var args = new[]
        {
            "Id is {0}",
            "123"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }

    [Fact]
    public async Task Format_with_multiple_arguments()
    {
        var args = new []
        {
            "{0} = {1}",
            "Key",
            "Value"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }

    [Fact]
    public async Task Format_with_multiple_arguments_including_a_formatted_integer()
    {
        var args = new[]
        {
            "{0,-12} = {1:0,000}",
            "Balance1",
            "1234567"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }

    [Fact]
    public async Task Format_with_multiple_arguments_including_a_formatted_decimal()
    {
        var args = new[]
        {
            "{0,-12} = {1:0,000.00}",
            "Balance1",
            "12345.67"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }

    [Fact]
    public async Task Format_with_multiple_arguments_including_a_formatted_date()
    {
        var args = new[]
        {
            "{0,-12} = {1:dd-MM-yyyy}",
            "Balance1",
            "2025-10-27"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }

    [Fact]
    public async Task Format_with_multiple_arguments_including_a_formatted_datetime()
    {
        var args = new[]
        {
            "{0,-12} = {1:dd-MM-yyyy hh:mm:ss.fff}",
            "Balance1",
            "2025-10-27 12:34:56.123"
        };

        // Act
        var result = await Program.Main(args);

        // Assert
        result.ShouldBe(0, ErrorText.ToString());
        AssertExpectedOutput();
    }
}
