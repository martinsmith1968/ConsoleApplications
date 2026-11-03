using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services.Generators;

internal class SequentialGUIDGenerator : IGUIDGenerator
{
    private static Guid? _previous;

    private static readonly int[] SqlOrderMap = { 3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10 };

    public GUIDGenerationModeType Type => GUIDGenerationModeType.Sequential;

    public void Reset()
    {
        _previous = null;
    }

    /// <remarks>
    /// From : https://stackoverflow.com/questions/1752004/sequential-guid-generator
    /// From : http://developmenttips.blogspot.com/2008/03/generate-sequential-guids-for-sql.html
    /// </remarks>
    public Guid Generate()
    {
        _previous ??= Guid.NewGuid();

        var bytes = _previous.Value.ToByteArray();
        for (var mapIndex = 0; mapIndex < 16; mapIndex++)
        {
            var bytesIndex = SqlOrderMap[mapIndex];
            bytes[bytesIndex]++;
            if (bytes[bytesIndex] != 0)
            {
                break; // No need to increment more significant bytes
            }
        }

        return new Guid(bytes);
    }
}
