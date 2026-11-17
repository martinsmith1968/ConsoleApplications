using System.Globalization;
using System.Text.RegularExpressions;

namespace Toaster;

/// <summary>
/// Expands dynamic placeholders embedded in toast Text/Title:
/// "{token}" / "{token:format}", e.g. "It is now {now:HH:mm:ss}", and "%VARIABLE%" environment variables,
/// e.g. "Hello %USERNAME%". Anything unrecognised is left untouched so ordinary text containing braces or
/// percent signs (e.g. "50% off") doesn't misfire.
/// </summary>
internal static partial class DynamicText
{
    private static readonly Dictionary<string, Func<DateTime>> ValueSources = new(StringComparer.OrdinalIgnoreCase)
    {
        ["now"] = () => DateTime.Now,
        ["utcnow"] = () => DateTime.UtcNow,
    };

    [GeneratedRegex(@"\{(?<token>[A-Za-z]+)(:(?<format>[^}]*))?\}")]
    private static partial Regex PlaceholderRegex();

    [GeneratedRegex(@"%(?<name>[A-Za-z_][A-Za-z0-9_]*)%")]
    private static partial Regex EnvironmentVariableRegex();

    public static string Resolve(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        text = PlaceholderRegex().Replace(text, ResolvePlaceholder);
        text = EnvironmentVariableRegex().Replace(text, ResolveEnvironmentVariable);

        return text;
    }

    private static string ResolvePlaceholder(Match match)
    {
        var token = match.Groups["token"].Value;
        if (!ValueSources.TryGetValue(token, out var valueSource))
            return match.Value;

        var format = match.Groups["format"].Success ? match.Groups["format"].Value : null;
        var value = valueSource();

        try
        {
            return string.IsNullOrEmpty(format)
                ? value.ToString(CultureInfo.CurrentCulture)
                : value.ToString(format, CultureInfo.CurrentCulture);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"'{format}' is not a valid format string for the '{{{token}}}' placeholder", ex);
        }
    }

    private static string ResolveEnvironmentVariable(Match match)
    {
        var name = match.Groups["name"].Value;

        return Environment.GetEnvironmentVariable(name) ?? match.Value;
    }
}
