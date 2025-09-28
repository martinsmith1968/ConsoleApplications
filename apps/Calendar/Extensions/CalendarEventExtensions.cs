using Spectre.Console;

namespace Calendar.Extensions;

public static class CalendarEventExtensions
{
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
