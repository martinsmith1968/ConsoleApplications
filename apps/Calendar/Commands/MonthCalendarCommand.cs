using System.ComponentModel;
using Calendar.Extensions;
using ConsoleApplications.Common.CommandLine;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Calendar.Commands;

public sealed class MonthCalendarCommand : Command<MonthCalendarCommand.Settings>
{
    public const string CommandName = "month";

    public sealed class Settings : CustomCommandSettings
    {
        [Description("The Year to generate a Month calendar for")]
        [CommandOption("-y|--year", isRequired: false)]
        public int? Year { get; set; }

        [Description("The Month to generate a Month calendar for")]
        [CommandOption("-m|--month", isRequired: false)]
        public int? Month { get; set; }

        [Description("Hide the header showing the Year and Month details")]
        [CommandOption("-h|--hide-header", isRequired: false)]
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
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var utcNow = DateTime.UtcNow.Date;
        var now = DateTime.Now.Date;

        var year = settings.Year ?? DateTime.UtcNow.Year;
        var month = settings.Month ?? DateTime.UtcNow.Month;

        var calendar = new Spectre.Console.Calendar(year, month);

        if (settings.HideHeader)
            calendar.HideHeader();

        if (!settings.NoHighlightToday)
        {
            calendar.AddCalendarEvent("Now", now, Style.Parse("yellow bold"));
            calendar.AddCalendarEvent("UTC Now", utcNow, Style.Parse("yellow bold"));
        }

        calendar.AddCalendarEvent("My Birthday", 1968, 8, 11, Style.Parse("blue bold"));

        AnsiConsole.Write(calendar);

        if (settings.ShowEventsTable)
        {
            var table = new Table();
            table.AddColumn("Date");
            table.AddColumn("Day");
            table.AddColumn("Event");

            foreach (var ev in calendar.CalendarEvents.Where(x => x.GetDate() >= calendar.GetStartDate() && x.GetDate() <= calendar.GetEndDate()))
            {
                var dt = ev.GetDate();

                table.AddRow(dt.ToString("yyyy-MM-dd"), dt.DayOfWeek.ToString(), ev.Description);
            }

            //table.AddRow(utcNow.ToString("yyyy-MM-dd"), utcNow.ToString("dddd"), "UTC Now");
            //if (now != utcNow)
            //    table.AddRow(now.ToString("yyyy-MM-dd"), now.ToString("dddd"), "Local Now");
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
        }

        return 0;
    }
}
