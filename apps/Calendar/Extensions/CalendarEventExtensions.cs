using Calendar.Services;
using DNX.Extensions.DateTimes;
using Spectre.Console;

namespace Calendar.Extensions;

public static class CalendarEventExtensions
{
    /// <summary>Adds the sample events shown by both the month and year commands.</summary>
    public static void AddDefaultEvents(this Spectre.Console.Calendar calendar, bool highlightToday)
    {
        if (highlightToday)
        {
            calendar.AddCalendarEvent("Now", DateTime.Now.Date, Style.Parse("yellow bold"));

            if (DateTime.UtcNow.Date != DateTime.Now.Date)
            {
                calendar.AddCalendarEvent("UTC Now", DateTime.UtcNow.Date, Style.Parse("yellow bold"));
            }
        }

        calendar.AddCalendarEvent("My Birthday", 1968, 8, 11, Style.Parse("blue bold"));
    }

    /// <summary>Adds events loaded from a CSV events file (see <see cref="EventsFileReader"/>).</summary>
    public static void AddEvents(this Spectre.Console.Calendar calendar, IEnumerable<CsvCalendarEvent> events)
    {
        foreach (var ev in events)
        {
            calendar.AddCalendarEvent(ev.Description, ev.Date, ev.Style);
        }
    }

    public static DateOnly GetDate(this CalendarEvent calendarEvent)
    {
        return new DateOnly(calendarEvent.Year, calendarEvent.Month, calendarEvent.Day);
    }

    public static DateOnly GetStartDate(this Spectre.Console.Calendar calendar)
    {
        return new DateOnly(calendar.Year, calendar.Month, 1);
    }

    public static DateOnly GetEndDate(this Spectre.Console.Calendar calendar)
    {
        return (calendar.GetStartDate()).AddMonths(1).AddDays(-1);
    }
}
