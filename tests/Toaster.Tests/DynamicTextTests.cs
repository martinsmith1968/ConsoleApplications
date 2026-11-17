namespace Toaster.Tests;

public class DynamicTextTests
{
    [Fact]
    public void Resolve_leaves_plain_text_unchanged()
    {
        DynamicText.Resolve("Hello World").ShouldBe("Hello World");
    }

    [Theory]
    [InlineData("")]
    public void Resolve_leaves_empty_text_unchanged(string text)
    {
        DynamicText.Resolve(text).ShouldBe(text);
    }

    [Fact]
    public void Resolve_substitutes_now_without_a_format()
    {
        var before = DateTime.Now;

        var result = DynamicText.Resolve("It is now {now}");

        var after = DateTime.Now;
        result.ShouldNotContain("{now}");
        DateTime.Parse(result["It is now ".Length..]).ShouldBeInRange(before.AddSeconds(-1), after.AddSeconds(1));
    }

    [Fact]
    public void Resolve_substitutes_now_with_a_format()
    {
        var result = DynamicText.Resolve("It is now {now:HH:mm:ss}");

        result.ShouldMatch(@"^It is now \d{2}:\d{2}:\d{2}$");
    }

    [Fact]
    public void Resolve_substitutes_utcnow_with_a_format()
    {
        var result = DynamicText.Resolve("UTC date: {utcnow:yyyy-MM-dd}");

        result.ShouldBe($"UTC date: {DateTime.UtcNow:yyyy-MM-dd}");
    }

    [Fact]
    public void Resolve_substitutes_multiple_placeholders()
    {
        var result = DynamicText.Resolve("{now:yyyy} / {utcnow:yyyy}");

        result.ShouldBe($"{DateTime.Now:yyyy} / {DateTime.UtcNow:yyyy}");
    }

    [Theory]
    [InlineData("now")]
    [InlineData("NOW")]
    [InlineData("Now")]
    public void Resolve_matches_token_case_insensitively(string token)
    {
        var result = DynamicText.Resolve($"{{{token}:yyyy}}");

        result.ShouldBe($"{DateTime.Now:yyyy}");
    }

    [Fact]
    public void Resolve_leaves_unrecognised_placeholder_untouched()
    {
        DynamicText.Resolve("Hello {name}").ShouldBe("Hello {name}");
    }

    [Fact]
    public void Resolve_throws_for_an_invalid_format()
    {
        Should.Throw<FormatException>(() => DynamicText.Resolve("{now:'unterminated}"));
    }

    [Fact]
    public void Resolve_substitutes_a_known_environment_variable()
    {
        var name = $"TOASTER_TEST_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(name, "SomeValue");
        try
        {
            DynamicText.Resolve($"Hello %{name}%").ShouldBe("Hello SomeValue");
        }
        finally
        {
            Environment.SetEnvironmentVariable(name, null);
        }
    }

    [Fact]
    public void Resolve_substitutes_multiple_environment_variables()
    {
        var firstName = $"TOASTER_TEST_{Guid.NewGuid():N}";
        var secondName = $"TOASTER_TEST_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(firstName, "First");
        Environment.SetEnvironmentVariable(secondName, "Second");
        try
        {
            DynamicText.Resolve($"%{firstName}% and %{secondName}%").ShouldBe("First and Second");
        }
        finally
        {
            Environment.SetEnvironmentVariable(firstName, null);
            Environment.SetEnvironmentVariable(secondName, null);
        }
    }

    [Fact]
    public void Resolve_leaves_unknown_environment_variable_untouched()
    {
        var name = $"TOASTER_TEST_UNDEFINED_{Guid.NewGuid():N}";

        DynamicText.Resolve($"Hello %{name}%").ShouldBe($"Hello %{name}%");
    }

    [Fact]
    public void Resolve_leaves_a_lone_percent_sign_untouched()
    {
        DynamicText.Resolve("50% off").ShouldBe("50% off");
    }

    [Fact]
    public void Resolve_substitutes_environment_variables_and_placeholders_together()
    {
        var name = $"TOASTER_TEST_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(name, "World");
        try
        {
            var result = DynamicText.Resolve($"Hello %{name}%, it is now {{now:yyyy}}");

            result.ShouldBe($"Hello World, it is now {DateTime.Now:yyyy}");
        }
        finally
        {
            Environment.SetEnvironmentVariable(name, null);
        }
    }
}
