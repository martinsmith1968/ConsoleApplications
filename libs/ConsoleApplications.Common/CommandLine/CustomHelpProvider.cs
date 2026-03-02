using DNX.Extensions.Assemblies;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

namespace ConsoleApplications.Common.CommandLine;

public class CustomHelpProvider(ICommandAppSettings settings)
    : HelpProvider(settings)
{
    /// <summary>
    /// Gets the header for the help information.
    /// </summary>
    /// <param name="model">The command model to write help for.</param>
    /// <param name="command">The command for which to write help information (optional).</param>
    /// <returns>
    /// An enumerable collection of <see cref="T:Spectre.Console.Rendering.IRenderable" /> objects.
    /// </returns>
    /// <remarks>
    /// See : https://spectreconsole.net/appendix/colors
    /// </remarks>
    public override IEnumerable<IRenderable> GetHeader(ICommandModel model, ICommandInfo? command)
    {
        var assemblyInfo = AssemblyDetails.ForEntryPoint();

        return
        [
            new Markup($"[Yellow]{assemblyInfo.Name}[/] v[dodgerblue1]{assemblyInfo.SimplifiedVersion}[/] - {assemblyInfo.Description}"), Text.NewLine,
            new Markup($"[grey]Copyright © {ReplaceDynamicTextValues(assemblyInfo.Copyright)}[/]"), Text.NewLine,
            Text.NewLine
        ];
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="model"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <remarks>
    /// Cloned from SpectreConsole and reordered
    /// </remarks>
    public override IEnumerable<IRenderable> Write(ICommandModel model, ICommandInfo? command)
    {
        // return base.Write(model, command);

        var result = new List<IRenderable>();

        result.AddRange(GetHeader(model, command));
        result.AddRange(GetDescription(model, command));
        result.AddRange(GetUsage(model, command));
        result.AddRange(GetCommands(model, command));
        result.AddRange(GetArguments(model, command));
        result.AddRange(GetOptions(model, command));
        result.AddRange(GetFooter(model, command));
        result.AddRange(GetExamples(model, command));

        return result;
    }

    private static string? ReplaceDynamicTextValues(string? text)
    {
        if (text == null)
            return text;

        var format = text
                .Replace("{Now", "{0")
                .Replace("{UtcNow", "{1")
            ;

        return string.Format(
                format,
                DateTime.Now,
                DateTime.UtcNow
            );
    }
}
