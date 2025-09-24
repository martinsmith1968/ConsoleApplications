using ConsoleApplications.Common.Services.Interfaces;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class BuiltInGUIDGenerator : IGUIDGenerator
{
    public Guid Generate()
    {
        return Guid.NewGuid();
    }
}
