using System.Reflection;
using ConsoleApplications.Common.Attributes;
using ConsoleApplications.Common.Services.Interfaces;
using ConsoleApplications.Common.Types;

namespace ConsoleApplications.Common.Services.GUIDGeneration;

public class GUIDGeneratorFactory
{
    public static IGUIDGenerator? Create(GUIDGeneratorType guidGeneratorType)
    {
        var candidates = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass)
            .Where(t => !t.IsAbstract)
            .Where(x => x.GetInterfaces().Contains(typeof(IGUIDGenerator)))
            .ToArray();

        var supported = candidates
            .Single(t =>
                t.GetCustomAttributes<SupportedGUIDGeneratorTypeAttribute>()
                    .Any(a => a.GUIDGeneratorType == guidGeneratorType)
                );

        return Activator.CreateInstance(supported) as IGUIDGenerator;
    }
}
