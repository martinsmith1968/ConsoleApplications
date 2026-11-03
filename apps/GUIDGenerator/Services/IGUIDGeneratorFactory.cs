using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services;
internal interface IGUIDGeneratorFactory
{
    IGUIDGenerator Create(GUIDGenerationModeType generationType);
}
