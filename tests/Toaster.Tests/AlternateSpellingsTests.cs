namespace Toaster.Tests;

public class AlternateSpellingsTests
{
    [Theory]
    [InlineData("Center", "Centre")]
    [InlineData("CENTER", "Centre")]
    [InlineData("center", "Centre")]
    public void ToCanonical_maps_known_alternate_spelling(string text, string expected)
    {
        AlternateSpellings.ToCanonical(text).ShouldBe(expected);
    }

    [Theory]
    [InlineData("Centre")]
    [InlineData("Left")]
    [InlineData("NotAKnownValue")]
    public void ToCanonical_leaves_unknown_text_unchanged(string text)
    {
        AlternateSpellings.ToCanonical(text).ShouldBe(text);
    }
}
