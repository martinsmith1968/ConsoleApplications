using ConsoleApplications.Common.Interfaces;
using ConsoleApplications.Common.Types;
using Ookii.CommandLine;

namespace ConsoleApplications.Common.CommandLine;

public class CustomParseOptionsOverride : ICustomParseOptionsOverride
{
    [CommandLineArgument(ShortName = '@', DefaultValue = true, Category = CommandLineCategoryType.OptionsFiles, Position = int.MaxValue - 1)]
    public bool UseDefaultArgumentsFile { get; set; }

    [CommandLineArgument(ShortName = '$', DefaultValue = true, Category = CommandLineCategoryType.OptionsFiles, Position = int.MaxValue)]
    public bool UseLocalArgumentsFile { get; set; }
}
