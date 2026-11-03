using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services.Generators;

internal class EntityFrameworkGUIDGenerator : IGUIDGenerator
{
    private static long _lastTicks = -1;

    public GUIDGenerationModeType Type => GUIDGenerationModeType.EntityFramework;

    public void Reset()
    {
        _lastTicks = -1;
    }

    public Guid Generate()
    {
        var ticks = DateTime.UtcNow.Ticks;

        if (ticks <= _lastTicks)
        {
            ticks = _lastTicks + 1;
        }
        _lastTicks = ticks;

        var ticksBytes = BitConverter.GetBytes(ticks);

        Array.Reverse(ticksBytes);

        var myGuid = new Guid();
        var guidBytes = myGuid.ToByteArray();

        Array.Copy(ticksBytes, 0, guidBytes, 10, 6);
        Array.Copy(ticksBytes, 6, guidBytes, 8, 2);

        var newGuid = new Guid(guidBytes);

        return newGuid;
    }
}
