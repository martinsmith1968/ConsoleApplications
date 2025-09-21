using System.ComponentModel;
using ConsoleApplications.Common.Interfaces;
using ConsoleApplications.Common.Types;
using Ookii.CommandLine;

namespace ConsoleApplications.Common.CommandLine;

[GeneratedParser]
public partial class CustomParseOptionsOverride : ICustomParseOptionsOverride
{
    [CommandLineArgument(ShortName = '@', DefaultValue = true, Category = CommandLineCategoryType.OptionsFiles, Position = int.MaxValue - 1)]
    [Description("Use the Argument File in the application directory, if present")]
    public bool UseDefaultArgumentsFile { get; set; } = true;

    [CommandLineArgument(ShortName = '$', DefaultValue = true, Category = CommandLineCategoryType.OptionsFiles, Position = int.MaxValue)]
    [Description("Use the Argument File in the local directory, if present")]
    public bool UseLocalArgumentsFile { get; set; } = true;
}
