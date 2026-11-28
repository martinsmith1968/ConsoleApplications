using System.ComponentModel;
using Calendar.Extensions;
using Calendar.Services;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;

namespace Calendar.Commands;

[Description("Show a grid of calendars covering a range of months in a year")]
public sealed class YearCalendarCommand : Command<YearCalendarCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => "year";

    public sealed class Settings : CommonCommandSettings
    {
        [Description("The Year to generate a Year calendar for")]
        [CommandOption("-y|--year", isRequired: false)]
        public int? Year { get; set; }

        [Description("The maximum number of months to place side by side per row (fewer are shown if the terminal is too narrow)")]
        [CommandOption("-w|--width", isRequired: false)]
        [DefaultValue(3)]
        public int Width { get; set; } = 3;

        [Description("The first month to show")]
        [CommandOption("-f|--first-month", isRequired: false)]
        [DefaultValue(1)]
        public int FirstMonth { get; set; } = 1;

        [Description("The total number of months to show")]
        [CommandOption("-t|--total-months", isRequired: false)]
        [DefaultValue(12)]
        public int TotalMonths { get; set; } = 12;

        [Description("Don't show a highlight for today (if in the shown calendars)")]
        [CommandOption("-n|--no-highlight-today", isRequired: false)]
        [DefaultValue(false)]
        public bool NoHighlightToday { get; set; }

        [Description("A CSV file of events to show (Date,Description,Category - Category is a Spectre.Console style, e.g. \"yellow bold\")")]
        [CommandOption("--ef|--events-file", isRequired: false)]
        public FileInfo? EventsFile { get; set; }

        public void CustomValidate()
        {
            if (Width < 1)
                throw new ArgumentOutOfRangeException(nameof(Width), "Value must be greater than zero");

            if (FirstMonth is < 1 or > 12)
                throw new ArgumentOutOfRangeException(nameof(FirstMonth), "Value must be between 1 and 12");

            if (TotalMonths < 1)
                throw new ArgumentOutOfRangeException(nameof(TotalMonths), "Value must be greater than zero");

            if (EventsFile is { Exists: false })
                throw new FileNotFoundException($"Events file not found: {EventsFile.FullName}", EventsFile.FullName);
        }
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>()
        {
            { [ GetCommandName(), "-y", "2000" ] },
            { [ GetCommandName(), "-y", "2000", "-f", "7" ] },
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

        var csvEvents = settings.EventsFile is not null
            ? EventsFileReader.Read(settings.EventsFile)
            : [];

        var calendars = new List<IRenderable>();

        var month = settings.FirstMonth;
        var monthsShown = 0;

        while (monthsShown < settings.TotalMonths && month <= 12)
        {
            var calendar = new Spectre.Console.Calendar(year, month);
            calendar.AddDefaultEvents(!settings.NoHighlightToday);
            calendar.AddEvents(csvEvents);

            calendars.Add(calendar);

            month++;
            monthsShown++;
        }

        foreach (var row in calendars.Chunk(settings.Width))
        {
            AnsiConsole.Write(new Columns(row));
        }

        return 0;
    }
}
