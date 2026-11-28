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
    /// Gets the options section of the help information.
    /// </summary>
    /// <param name="model">The command model to write help for.</param>
    /// <param name="command">The command for which to write help information (optional).</param>
    /// <returns>
    /// An enumerable collection of <see cref="T:Spectre.Console.Rendering.IRenderable" /> objects.
    /// </returns>
    /// <remarks>
    /// Cloned from Spectre.Console's own <see cref="HelpProvider"/>, because its version only ever shows the
    /// first short name and first long name of an option (<c>ShortNames.FirstOrDefault()</c> /
    /// <c>LongNames.FirstOrDefault()</c>) - any further aliases (e.g. "--ef|--events-file") are silently
    /// dropped from the rendered help, even though they still work as real arguments. This override lists
    /// every alias instead.
    /// </remarks>
    public override IEnumerable<IRenderable> GetOptions(ICommandModel model, ICommandInfo? command)
    {
        var rows = GetOptionRows(model, command);
        if (rows.Count == 0)
            return [];

        var showDefaults = ShowOptionDefaultValues && rows.Any(r => r.DefaultValue is not null);

        var grid = new Grid();
        grid.AddColumn(new GridColumn { Padding = new Padding(4, 4), NoWrap = true });
        if (showDefaults)
            grid.AddColumn(new GridColumn { Padding = new Padding(0, 0, 4, 0) });
        grid.AddColumn(new GridColumn { Padding = new Padding(0, 0) });

        if (showDefaults)
        {
            var defaultValueHeaderStyle = _helpProviderStyles?.Options?.DefaultValueHeader ?? Style.Plain;

            grid.AddRow(new Markup(" "), new Markup("DEFAULT", defaultValueHeaderStyle), new Markup(" "));
        }

        foreach (var row in rows)
        {
            var cells = new List<IRenderable> { new Markup(BuildOptionNamesMarkup(row)) };

            if (showDefaults)
                cells.Add(BuildDefaultValueMarkup(row.DefaultValue));

            var description = row.Description;
            if (row.IsRequired)
            {
                description = string.IsNullOrWhiteSpace(description)
                    ? "[i]Required[/]"
                    : description.TrimEnd('.') + ". [i]Required[/]";
            }

            cells.Add(new Markup(NormalizeDescription(description)));

            grid.AddRow(cells.ToArray());
        }

        return
        [
            Text.NewLine,
            new Markup("OPTIONS:", _helpProviderStyles?.Options?.Header ?? Style.Plain),
            Text.NewLine,
            grid,
        ];
    }

    /// <summary>A single row in the options help table - every alias, not just the first of each kind.</summary>
    private sealed record OptionRow(
        IReadOnlyList<string> ShortNames,
        IReadOnlyList<string> LongNames,
        string? ValueName,
        bool ValueIsOptional,
        bool IsRequired,
        string? Description,
        object? DefaultValue);

    private static IReadOnlyList<OptionRow> GetOptionRows(ICommandModel model, ICommandInfo? command)
    {
        var rows = new List<OptionRow>
        {
            new(["h"], ["help"], null, false, false, "Prints help information", null),
        };

        if (command?.Parent is null && (command is null || !command.IsBranch) && (command is null || command.IsDefaultCommand))
        {
            var hasVersionOption = command?.Parameters.OfType<ICommandOption>()
                .Any(o => o.ShortNames.Any(v => v.Equals("v", StringComparison.OrdinalIgnoreCase))
                          || o.LongNames.Any(v => v.Equals("version", StringComparison.OrdinalIgnoreCase))) ?? false;

            if (!hasVersionOption && model.ApplicationVersion is not null)
                rows.Add(new OptionRow(["v"], ["version"], null, false, false, "Prints version information", null));
        }

        if (command is not null)
        {
            foreach (var option in command.Parameters.OfType<ICommandOption>().Where(o => !o.IsHidden))
            {
                var defaultValue = option.IsFlag && option.DefaultValue?.Value is false
                    ? null
                    : option.DefaultValue?.Value;

                rows.Add(new OptionRow(option.ShortNames, option.LongNames, option.ValueName, option.ValueIsOptional, option.IsRequired, option.Description, defaultValue));
            }
        }

        return rows;
    }

    private string BuildOptionNamesMarkup(OptionRow row)
    {
        var names = string.Join(", ", row.ShortNames.Select(s => $"-{s}").Concat(row.LongNames.Select(l => $"--{l}")));

        if (row.ValueName is null)
            return names;

        var valueStyle = row.ValueIsOptional
            ? _helpProviderStyles?.Options?.OptionalOptionValue ?? Style.Plain
            : _helpProviderStyles?.Options?.RequiredOptionValue ?? Style.Plain;

        var valueText = row.ValueIsOptional ? $"[{row.ValueName}]" : $"<{row.ValueName}>";

        return $"{names} [{valueStyle.ToMarkup()}]{valueText.EscapeMarkup()}[/]";
    }

    private Markup BuildDefaultValueMarkup(object? defaultValue)
    {
        var style = _helpProviderStyles?.Options?.DefaultValue ?? Style.Plain;

        switch (defaultValue)
        {
            case null:
            case string { Length: 0 }:
                return new Markup(" ");
            case Array { Length: 0 }:
                return new Markup(" ");
            case Array array:
                var joined = string.Join(", ", array.Cast<object>().Select(o => (o.ToString() ?? string.Empty).EscapeMarkup()));
                return new Markup(joined, style);
            default:
                return new Markup((defaultValue.ToString() ?? string.Empty).EscapeMarkup(), style);
        }
    }

    private string NormalizeDescription(string? description)
    {
        if (description is null)
            return " ";

        return TrimTrailingPeriod ? description.TrimEnd('.') : description;
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
