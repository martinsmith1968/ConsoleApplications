using System.ComponentModel;
using System.Reflection;
using Spectre.Console.Cli;

namespace ConsoleApplications.Common.CommandLine;

public class CustomCommandSettings : CommandSettings
{
    public const string DefaultArgumentsFileExtension = "options";

    [Description("Control whether to read the Default arguments file (if present)")]
    [CommandOption("--ignore-default-arguments-file", isRequired: false, IsHidden = false)]
    [DefaultValue(false)]
    public bool IgnoreDefaultArgumentsFile { get; set; }

    [Description("Control whether to read the Local arguments file (if present)")]
    [CommandOption("--ignore-local-arguments-file", isRequired: false, IsHidden = false)]
    [DefaultValue(false)]
    public bool IgnoreLocalArgumentsFile { get; set; }

    public string ArgumentsFileNameOnly { get; set; } = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly()?.Location ?? string.Empty);
    public string ArgumentsFileExtension { get; set; } = DefaultArgumentsFileExtension;

    public string GetArgumentsFileName() => $"{ArgumentsFileNameOnly.Trim('.')}.{ArgumentsFileExtension.Trim('.')}";
    public FileInfo GetDefaultArgumentsFileName() => new(Path.Combine(Assembly.GetEntryAssembly()?.Location ?? string.Empty, $"{GetArgumentsFileName()}"));
    public FileInfo GetLocalArgumentsFileName() => new(Path.Combine(Directory.GetCurrentDirectory(), $"{GetArgumentsFileName()}"));
}
