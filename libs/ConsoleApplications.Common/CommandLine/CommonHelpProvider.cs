using System.Reflection;
using ConsoleApplications.Common.Interfaces;
using DNX.Extensions.Assemblies;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

namespace ConsoleApplications.Common.CommandLine;

public class CommonHelpProvider : HelpProvider
{
    private readonly HelpProviderStyle? _helpProviderStyles;

    public CommonHelpProvider(ICommandAppSettings settings)
        : base(settings)
    {
        _helpProviderStyles = settings.HelpProviderStyles;
    }

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
        result.AddRange(GetExamples(model, command));
        result.AddRange(GetFooter(model, command));

        return result;
    }

    /// <summary>
    /// Gets the footer for the help information, including any notes contributed by an <see cref="ICustomCommand"/>.
    /// </summary>
    /// <param name="model">The command model to write help for.</param>
    /// <param name="command">The command for which to write help information (optional).</param>
    /// <returns>
    /// An enumerable collection of <see cref="T:Spectre.Console.Rendering.IRenderable" /> objects.
    /// </returns>
    /// <remarks>
    /// <paramref name="command"/> is Spectre's own internal, sealed <c>CommandInfo</c> metadata object, not the
    /// <see cref="ICommand"/> instance that gets executed - so it can't be pattern-matched against
    /// <see cref="ICustomCommand"/> directly. Its <c>CommandType</c> property is public, so it's read via
    /// reflection (without naming the internal declaring type), and <see cref="ICustomCommand.GetNotes"/> -
    /// itself a static interface member - is likewise invoked via reflection rather than an instance.
    /// </remarks>
    public override IEnumerable<IRenderable> GetFooter(ICommandModel model, ICommandInfo? command)
    {
        var result = new List<IRenderable>();

        var notes = GetNotes(command);
        if (notes.Length > 0)
        {
            // No dedicated "Notes" slot exists on HelpProviderStyle, so this borrows the Options
            // header style, honouring whatever style the app (or its HelpProviderStyles) configured.
            var headerStyle = _helpProviderStyles?.Options?.Header ?? Style.Plain;

            result.Add(Text.NewLine);
            result.Add(new Markup("NOTES:", headerStyle));
            result.Add(Text.NewLine);

            foreach (var note in notes)
            {
                result.Add(new Markup($"    {note.EscapeMarkup()}"));
                result.Add(Text.NewLine);
            }
        }

        result.AddRange(base.GetFooter(model, command));

        return result;
    }

    private static string[] GetNotes(ICommandInfo? command)
    {
        try
        {
            var commandType = command?.GetType()
                .GetProperty("CommandType", BindingFlags.Public | BindingFlags.Instance)?
                .GetValue(command) as Type;

            if (commandType is null || !typeof(ICustomCommand).IsAssignableFrom(commandType))
                return [];

            var getNotes = commandType.GetMethod(nameof(ICustomCommand.GetNotes), BindingFlags.Public | BindingFlags.Static);

            return getNotes?.Invoke(null, null) as string[] ?? [];
        }
        catch
        {
            return [];
        }
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
