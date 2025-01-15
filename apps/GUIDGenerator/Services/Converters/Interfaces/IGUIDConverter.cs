namespace GUIDGenerator.Services.Converters.Interfaces;

public interface IGUIDConverter
{
    /// <summary>
    /// Generates a deterministic Guid from Text
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    /// <remarks>
    /// From : https://stackoverflow.com/questions/30092463/hash-function-to-generate-16-alphanumerical-characters-from-input-string-in-c-sh
    /// </remarks>
    Guid GenerateFrom(string text);
}
