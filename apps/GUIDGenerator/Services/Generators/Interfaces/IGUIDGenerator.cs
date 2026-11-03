using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services.Generators.Interfaces;

public interface IGUIDGenerator
{
    GUIDGenerationModeType Type { get; }
    Guid Generate();
    void Reset();
}
