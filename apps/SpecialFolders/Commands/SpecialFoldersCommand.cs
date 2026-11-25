using System.ComponentModel;
using SpecialFolders.Configuration.Types;
using SpecialFolders.Writers;
using Spectre.Console.Cli;

namespace SpecialFolders.Commands;

[Description("Lists Windows special folder names and their resolved paths")]
public sealed class SpecialFoldersCommand : Command<SpecialFoldersCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The Special Folder to show")]
        [CommandArgument(0, "[name]")]
        public Environment.SpecialFolder? Name { get; set; }

        [Description("Collapse the Folder Names (Do not align)")]
        [CommandOption("-c|--CollapseNames")]
        public bool CollapseNames { get; set; }

        [Description("Hide the Item with an empty folder location")]
        [CommandOption("--HideEmptyLocation")]
        public bool HideEmptyLocation { get; set; }

        [Description("Sort the results")]
        [CommandOption("-s|--SortBy")]
        [DefaultValue(SortKey.Id)]
        public SortKey SortBy { get; set; }

        [Description("The output writer to use")]
        [CommandOption("-w|--Writer")]
        [DefaultValue(OutputWriterType.Standard)]
        public OutputWriterType Writer { get; set; }

        [Description("Show line numbers in the output ?")]
        [CommandOption("-l|--ShowLineNumbers")]
        public bool ShowLineNumbers { get; set; }

        [Description("Show the internal Special Folder Id in the output ?")]
        [CommandOption("-i|--ShowFolderId")]
        public bool ShowFolderId { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var specialFolders = Enum.GetValues<Environment.SpecialFolder>()
            .Distinct()
            .ToDictionary(
                sf => sf,
                Environment.GetFolderPath
            );

        var specialFoldersList = specialFolders
                .Where(sf => string.IsNullOrWhiteSpace(settings.Name?.ToString())
                             || sf.Key == settings.Name
                    )
                .Where(sf => !settings.HideEmptyLocation || !string.IsNullOrWhiteSpace(sf.Value))
                .ToList();

        specialFoldersList = settings.SortBy switch
        {
            SortKey.Id => specialFoldersList.OrderBy(sf => (int)sf.Key).ToList(),
            SortKey.Name => specialFoldersList.OrderBy(sf => sf.Key.ToString()).ToList(),
            SortKey.Location => specialFoldersList.OrderBy(sf => sf.Value).ToList(),
            _ => specialFoldersList
        };

        var writer = WriterFactory.CreateWriter(settings.Writer);

        writer.WriteOutputAsync(specialFoldersList, settings).GetAwaiter().GetResult();

        return 0;
    }
}
