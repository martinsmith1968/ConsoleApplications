using System.Reflection;

namespace TomlFileHandler.Tests.Extensions;

public static class AssemblyExtensions
{
    public static string GetEmbeddedResourceText(this Assembly assembly, string relativeName)
    {
        var names = assembly.GetManifestResourceNames();

        var resourceName = string.Join('.', assembly.GetName().Name, relativeName);

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return null;

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
