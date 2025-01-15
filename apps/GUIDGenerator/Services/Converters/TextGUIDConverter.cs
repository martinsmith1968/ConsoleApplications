using System.Security.Cryptography;
using System.Text;
using GUIDGenerator.Services.Converters.Interfaces;

namespace GUIDGenerator.Services.Converters;

public class TextGUIDConverter : IGUIDConverter
{
    /// <summary>
    /// Generates a deterministic Guid from Text
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    /// <remarks>
    /// From : https://stackoverflow.com/questions/30092463/hash-function-to-generate-16-alphanumerical-characters-from-input-string-in-c-sh
    /// </remarks>
    public Guid GenerateFrom(string text)
    {
        using var provider = MD5.Create();

        var inputBytes = Encoding.Default.GetBytes(text);

        var hashBytes = provider.ComputeHash(inputBytes);

        var hashGuid = new Guid(hashBytes);

        return hashGuid;
    }
}
