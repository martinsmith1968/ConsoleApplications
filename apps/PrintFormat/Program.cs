using Ookii.CommandLine;
using PrintFormat.Configuration;

namespace PrintFormat;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var arguments = CommandLineParser.Parse<Arguments>(args, Arguments.Options)
                            ?? throw new Exception("Unable to Parse Command Line");
            arguments.Validate();

            await Process(arguments);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }

    private static async Task Process(Arguments arguments)
    {
        var args = arguments.FormatArguments
            .Select(GetBestType)
            .ToArray();
        var text = string.Format(arguments.Format ?? string.Empty, args);

        await Console.Out.WriteLineAsync(text);
    }

    private static object GetBestType(string value)
    {
        if (decimal.TryParse(value, out var decimalResult))
            return decimalResult;

        if (int.TryParse(value, out var intResult))
            return intResult;

        if (DateOnly.TryParse(value, out var dateOnlyValue))
            return dateOnlyValue;

        if (DateTime.TryParse(value, out var dateTimeValue))
            return dateTimeValue;

        return value;
    }
}
