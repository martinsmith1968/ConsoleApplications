using DNX.Extensions.Assemblies;
using Spectre.Console.Cli;

namespace ConsoleApplications.Common.CommandLine;

public class CustomCommandAppConfiguration
{
    public static void Configure(IConfigurator config)
    {
        var assemblyInfo = AssemblyDetails.ForEntryPoint();

        //config.Settings.HelpProviderStyles = null;

        config.SetApplicationName(assemblyInfo.Name);
        config.PropagateExceptions();
        config.ValidateExamples();
        config.UseStrictParsing();
        config.CaseSensitivity(CaseSensitivity.All);
        config.SetHelpProvider(new CustomHelpProvider(config.Settings));
    }
}
