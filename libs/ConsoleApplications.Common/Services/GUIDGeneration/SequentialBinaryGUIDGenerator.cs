using ConsoleApplications.Common.Services.Interfaces;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class SequentialBinaryGUIDGenerator : IGUIDGenerator
{
    public Guid Generate()
    {
        return SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAsBinary);
    }
}
