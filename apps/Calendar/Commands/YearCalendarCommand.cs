using System.ComponentModel;
using ConsoleApplications.Common.CommandLine;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Calendar.Commands;

public sealed class YearCalendarCommand : Command<YearCalendarCommand.Settings>
{
    public const string CommandName = "year";

    public sealed class Settings : CustomCommandSettings
    {
        [Description("The Year to generate a Year calendar for")]
        [CommandOption("-y|--year", isRequired: false)]
        public int? Year { get; set; }

        [Description("How many widths width to draw")]
        [CommandOption("-w|--width", isRequired: false)]
        [DefaultValue(3)]
        public int Width { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings)
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


        for (var month = 1; month <= 12; month += settings.Width)
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
