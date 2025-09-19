using ConsoleApplications.Common.Services.Interfaces;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class SequentialAtEndGUIDGenerator : IGUIDGenerator
{
    public Guid Generate()
    {
        return SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAtEnd);
    }
}
