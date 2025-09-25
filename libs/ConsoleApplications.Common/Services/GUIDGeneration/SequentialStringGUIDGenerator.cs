using ConsoleApplications.Common.Services.Interfaces;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class SequentialStringGUIDGenerator : IGUIDGenerator
{
    public Guid Generate()
    {
        return SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAsString);
    }
}
