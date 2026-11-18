using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Calendar.Commands;

public sealed class YearCalendarCommand : Command<YearCalendarCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => "year";

    public sealed class Settings : CommonCommandSettings
    {
        [Description("The Year to generate a Year calendar for")]
        [CommandOption("-y|--year", isRequired: false)]
        public int? Year { get; set; }

        [Description("How many months width to draw")]
        [CommandOption("-w|--width", isRequired: false)]
        [DefaultValue(3)]
        public int Width { get; set; }

        [Description("The first month to show")]
        [CommandOption("-f|--first-month", isRequired: false)]
        [DefaultValue(1)]
        public int FirstMonth { get; set; }

        [Description("The total number of months to show")]
        [CommandOption("-t|--total-months", isRequired: false)]
        [DefaultValue(12)]
        [Range(1, 12)]
        public int TotalMonths { get; set; }
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>()
        {
            { [ GetCommandName(), "-y", "2000" ] },
            { [ GetCommandName(), "-y", "2000", "-f", "7" ] },
        };
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow.Date;
        var now = DateTime.Now.Date;

        var year = settings.Year ?? DateTime.UtcNow.Year;



        var table = new Table()
        {
            //Expand = true

        };

        for (var x = 0; x < settings.Width; x++)
        {
            table.AddColumn(new TableColumn("").Centered());
        }


        for (var month = settings.FirstMonth; month <= settings.TotalMonths; month += settings.Width)
        {
            var row = new List<IRenderable>();
            for (var x = 0; x < settings.Width; x++)
            {
                if (month + x <= 12)
                {
                    var calendar = new Spectre.Console.Calendar(year, month + x);
                    //calendar.HideHeader();
                    calendar.AddCalendarEvent("My Birthday", 1968, 8, 11, Style.Parse("blue bold"));
                    calendar.AddCalendarEvent("Now", now, Style.Parse("yellow bold"));
                    calendar.AddCalendarEvent("UTC Now", utcNow, Style.Parse("yellow bold"));
                    row.Add(calendar);
                }
                else
                {
                    row.Add(new Markup(""));
                }
            }
            table.AddRow(row);
        }





        AnsiConsole.Write(table);


        return 0;
    }
}
