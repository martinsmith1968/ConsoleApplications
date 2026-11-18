using System.Collections;
using System.ComponentModel;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace VarList.Commands;

public enum SortOrder
{
    None = 0,
    Name,
    Value
}

public enum OutputFormat
{
    Grid = 0,
    Table
}

public class VarListCommand : Command<VarListCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => CommonCommandApp.DeriveCommandName<VarListCommand>();

    public sealed class Settings : CommandSettings
    {
        [CommandOption("-s|--search")]
        public string SearchWildcard { get; set; }

        [CommandOption("-o|--order")]
        [DefaultValue(SortOrder.Name)]
        public SortOrder Order { get; set; }

        [CommandOption("-t|--target")]
        [DefaultValue(EnvironmentVariableTarget.User)]
        public EnvironmentVariableTarget Target { get; set; }

        [CommandOption("-f|--format")]
        [DefaultValue(OutputFormat.Grid)]
        public OutputFormat Format { get; set; }
    }

    private static IList<KeyValuePair<string, string>> GetVariables(EnvironmentVariableTarget target, SortOrder order)
    {
        var list = new List<KeyValuePair<string, string>>();

        var values = Environment.GetEnvironmentVariables(target);
        foreach (DictionaryEntry variable in values)
        {
            list.Add(
                new KeyValuePair<string, string>(Convert.ToString(variable.Key), Convert.ToString(variable.Value))
                );
        }

        return order switch
        {
            SortOrder.None => list,
            SortOrder.Name => list.OrderBy(x => x.Key).ToList(),
            SortOrder.Value => list.OrderBy(x => x.Value).ToList(),
            _ => list
        };
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var variables = GetVariables(settings.Target, settings.Order);

        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Value");

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        grid.AddRow("Name", "Value");

        PopulateData(variables, table, grid);

        AnsiConsole.Write(table);
        AnsiConsole.Write(grid);

        return 0;
    }

    private static void PopulateData(IList<KeyValuePair<string, string>> variables, Table table, Grid grid)
    {
        foreach (var variable in variables)
        {
            //Console.Out.WriteLine($"{variable.Key}: {variable.Value}");
            //AnsiConsole.MarkupLine($"[yellow]{variable.Key}[/]: {variable.Value}");

            table.AddRow($"[yellow]{variable.Key}[/]", $"{variable.Value}");

            grid.AddRow($"{variable.Key}", $"{variable.Value}");
        }
    }
}
