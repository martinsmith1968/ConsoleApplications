using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services;

public class GUIDGeneratorFactory : IGUIDGeneratorFactory
{
    private readonly IEnumerable<IGUIDGenerator> _generators;

    public GUIDGeneratorFactory(IEnumerable<IGUIDGenerator> generators)
    {
        _generators = generators;
    }

    public IGUIDGenerator Create(GUIDGenerationModeType generationType)
    {
        return _generators.FirstOrDefault(g => g.Type == generationType)
               ?? throw new ArgumentOutOfRangeException(nameof(generationType), $"Unknown or unsupported {nameof(GUIDGenerationModeType)} : {generationType}");
    }
}
