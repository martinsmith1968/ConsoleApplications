using System.Diagnostics;
using ConsoleApplications.Common.Services.Interfaces;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class DateTimeOffsetGUIDGenerator : IGUIDGenerator
{
    private static DateTimeOffset _lastTime = DateTimeOffset.MinValue;
    private static int _lastModifier = 0;

    public Guid Generate()
    {
        var timestamp = DateTimeOffset.UtcNow;
        Debug.WriteLine($"{nameof(timestamp)}: {timestamp:O}");

        if (timestamp <= _lastTime)
        {
            _lastModifier++;
        }
        else
        {
            _lastTime = timestamp;
            _lastModifier = 0;
        }
        Debug.WriteLine($"{nameof(_lastTime)}: {_lastTime:O}");
        Debug.WriteLine($"{nameof(_lastModifier)}: {_lastModifier}");

        var timestampText = _lastTime.ToString("yyyyMMddHHmmssfffffff")
                            + _lastModifier.ToString("0")
            ;
        Debug.WriteLine($"{nameof(timestampText)}: {timestampText}");

        var base64 = timestampText
            .Replace("_", "/")
            .Replace("-", "+") + "==";

        var blob = Convert.FromBase64String(base64);
        Debug.WriteLine($"{nameof(blob)}: {string.Join("-", blob.Select(b => b.ToString("X2")))}");
        var guid = new Guid(blob);

        return guid;
    }
}
