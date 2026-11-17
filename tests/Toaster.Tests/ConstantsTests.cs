namespace Toaster.Tests;

public class ConstantsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("auto", true)]
    [InlineData("AUTO", true)]
    [InlineData("360", false)]
    public void IsAuto_detects_auto_or_empty(string? text, bool expected)
    {
        Constants.IsAuto(text).ShouldBe(expected);
    }
}
