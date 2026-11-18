using DNX.Extensions.Assemblies;
using DNX.Extensions.Strings;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ConsoleApplications.Common.CommandLine;

public class CommonCommandApp
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
        config.SetHelpProvider(new CommonHelpProvider(config.Settings));
    }

    public static void DisplayError(string text)
    {
        AnsiConsole.MarkupLine($"[red]ERROR[/]: [yellow]{text}[/]");
    }

    public static void DisplayError(Exception ex, ExceptionFormats formats = ExceptionFormats.ShortenEverything)
    {
        AnsiConsole.WriteException(ex, formats);
    }

    public static string DeriveCommandName<T>()
    {
        return nameof(T).RemoveEndsWith(nameof(Command));
    }
}
