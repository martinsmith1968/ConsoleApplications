namespace Toaster;

public static class Constants
{
    public const string Auto = nameof(Auto);

    /// <summary>True if no explicit value was given (empty, or the literal "Auto") and a value should be derived automatically.</summary>
    public static bool IsAuto(string? text) =>
        string.IsNullOrWhiteSpace(text) || text.Trim().Equals(Auto, StringComparison.OrdinalIgnoreCase);
}
