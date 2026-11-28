using System.ComponentModel;
using Calendar.Extensions;
using Calendar.Services;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Calendar.Commands;

[Description("Show a calendar for a single month")]
public sealed class MonthCalendarCommand : Command<MonthCalendarCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => "month";

    public sealed class Settings : CommonCommandSettings
    {
        [Description("The Year to generate a Month calendar for")]
        [CommandOption("-y|--year", isRequired: false)]
        public int? Year { get; set; }

        [Description("The Month to generate a Month calendar for")]
        [CommandOption("-m|--month", isRequired: false)]
        public int? Month { get; set; }

        [Description("Hide the header showing the Year and Month details")]
        [CommandOption("--hide-header", isRequired: false)]
        [DefaultValue(false)]
        public bool HideHeader { get; set; }

        [Description("Don't show a highlight for today (if in the shown calendar)")]
        [CommandOption("-n|--no-highlight-today", isRequired: false)]
        [DefaultValue(false)]
        public bool NoHighlightToday { get; set; }

        [Description("Show a table of events")]
        [CommandOption("-e|--events-table", isRequired: false)]
        [DefaultValue(false)]
        public bool ShowEventsTable { get; set; }

        [Description("A CSV file of events to show (Date,Description,Category - Category is a Spectre.Console style, e.g. \"yellow bold\")")]
        [CommandOption("--ef|--events-file", isRequired: false)]
        public FileInfo? EventsFile { get; set; }

        public void CustomValidate()
        {
            if (Month is < 1 or > 12)
                throw new ArgumentOutOfRangeException(nameof(Month), "Value must be between 1 and 12");

            if (EventsFile is { Exists: false })
                throw new FileNotFoundException($"Events file not found: {EventsFile.FullName}", EventsFile.FullName);
        }
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>()
        {
            { [ GetCommandName(), "-m", "7" ] },
        };
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        try
        {
            settings.CustomValidate();

            return base.Validate(context, settings);
        }
        catch (Exception ex)
        {
            return ValidationResult.Error(ex.Message);
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var year = settings.Year ?? DateTime.UtcNow.Year;
        var month = settings.Month ?? DateTime.UtcNow.Month;

        var calendar = new Spectre.Console.Calendar(year, month);

        if (settings.HideHeader)
            calendar.HideHeader();

        calendar.AddDefaultEvents(!settings.NoHighlightToday);

        if (settings.EventsFile is not null)
            calendar.AddEvents(EventsFileReader.Read(settings.EventsFile));

        AnsiConsole.Write(calendar);

        if (settings.ShowEventsTable)
        {
            var table = new Table();
            table.AddColumn("Date");
            table.AddColumn("Day");
            table.AddColumn("Event");

            var events = calendar.CalendarEvents
                .Where(x => x.GetDate() >= calendar.GetStartDate() && x.GetDate() <= calendar.GetEndDate());

            foreach (var ev in events)
            {
                var dt = ev.GetDate();

                table.AddRow(dt.ToString("yyyy-MM-dd"), dt.DayOfWeek.ToString(), ev.Description);
            }

            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
        }

        return 0;
    }
}
