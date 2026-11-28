using System.Globalization;
using System.Text;
using Spectre.Console;

namespace Calendar.Services;

/// <summary>An event loaded from a CSV events file. <see cref="Style"/> comes straight from the CSV's
/// Category column, parsed as a Spectre.Console style string (e.g. "yellow bold").</summary>
public sealed record CsvCalendarEvent(DateTime Date, string Description, Style? Style);

/// <summary>Reads calendar events from a CSV file with columns Date,Description,Category.</summary>
public static class EventsFileReader
{
    public static IReadOnlyList<CsvCalendarEvent> Read(FileInfo file)
    {
        if (!file.Exists)
            throw new FileNotFoundException($"Events file not found: {file.FullName}", file.FullName);

        var events = new List<CsvCalendarEvent>();
        var isFirstLine = true;

        foreach (var line in File.ReadLines(file.FullName))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var fields = SplitLine(line);

            if (isFirstLine)
            {
                isFirstLine = false;

                if (fields.Count > 0 && fields[0].Equals("Date", StringComparison.OrdinalIgnoreCase))
                    continue;
            }

            if (fields.Count < 2)
                throw new FormatException($"Invalid events file line (expected Date,Description[,Category]): '{line}'");

            var date = DateTime.Parse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.None);
            var description = fields[1];
            var category = fields.Count > 2 ? fields[2] : string.Empty;
            var style = string.IsNullOrWhiteSpace(category) ? (Style?)null : Style.Parse(category);

            events.Add(new CsvCalendarEvent(date, description, style));
        }

        return events;
    }

    private static List<string> SplitLine(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var c in line)
        {
            if (inQuotes)
            {
                if (c == '"')
                    inQuotes = false;
                else
                    current.Append(c);
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                fields.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString().Trim());

        return fields;
    }
}
