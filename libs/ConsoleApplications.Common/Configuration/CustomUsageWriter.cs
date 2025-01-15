using System.Reflection;
using DNX.Helpers.Assemblies;
using Ookii.CommandLine;

namespace ConsoleApplications.Common.Configuration;

public class CustomUsageWriter : UsageWriter
{
    public CustomUsageWriter()
        : base(LineWrappingTextWriter.ForConsoleError(), true)
    {
        IncludeAliasInDescription                      = true;
        IncludeApplicationDescription                  = true;
        IncludeApplicationDescriptionBeforeCommandList = true;
        IncludeCommandHelpInstruction                  = true;
        IncludeCommandAliasInCommandList               = true;
        IncludeDefaultValueInDescription               = true;
        IncludeValidatorsInDescription                 = true;
    }

    private void WriteApplicationHeaderDetails(string description)
    {
        var assemblyDetails = new AssemblyDetails(Assembly.GetEntryAssembly());

        if (string.IsNullOrWhiteSpace(description))
            description = assemblyDetails.Description;

        Writer.WriteLine($"{ExecutableName} v{assemblyDetails.Version.Simplify(2)} - {description}");

        if (!string.IsNullOrWhiteSpace(assemblyDetails.Copyright))
            Writer.WriteLine(assemblyDetails.Copyright);

        Writer.WriteLine();
    }

    protected override void WriteApplicationDescription(string description)
    {
        WriteApplicationHeaderDetails(description);
    }
}
