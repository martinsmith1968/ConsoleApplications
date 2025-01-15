using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services.Generators;

public class StandardGUIDGenerator : IGUIDGenerator
{
    public GUIDGenerationModeType Type => GUIDGenerationModeType.Standard;

    public void Reset()
    {
    }

    public Guid Generate()
    {
        return Guid.NewGuid();
    }
}
