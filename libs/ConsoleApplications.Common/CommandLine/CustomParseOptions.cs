using System.Reflection;
using Ookii.CommandLine;

namespace ConsoleApplications.Common.CommandLine;

public class CustomParseOptions : ParseOptions
{
    public const string DefaultArgumentsFileExtension = "options";

    public bool UseDefaultArgumentsFile { get; set; } = true;
    public bool UseLocalArgumentsFile { get; set; } = true;

    public string ArgumentsFileNameOnly { get; set; } = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location ?? string.Empty);
    public string ArgumentsFileExtension { get; set; } = DefaultArgumentsFileExtension;

    public string GetArgumentsFileName() => $"{ArgumentsFileNameOnly.Trim('.')}.{ArgumentsFileExtension.Trim('.')}";
    public FileInfo GetDefaultArgumentsFileName() => new(Path.Combine(Assembly.GetEntryAssembly()?.Location ?? string.Empty, $"{GetArgumentsFileName()}"));
    public FileInfo GetLocalArgumentsFileName() => new(Path.Combine(Directory.GetCurrentDirectory(), $"{GetArgumentsFileName()}"));
}
