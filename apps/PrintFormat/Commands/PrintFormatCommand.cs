using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace PrintFormat.Commands;

[Description("Formats a .NET composite format string against supplied arguments")]
public sealed class PrintFormatCommand : Command<PrintFormatCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The Format string to use")]
        [CommandArgument(0, "<format>")]
        public string Format { get; set; } = string.Empty;

        [Description("The format arguments")]
        [CommandArgument(1, "<arguments>")]
        public string[] FormatArguments { get; set; } = [];

        public override ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Format))
                return ValidationResult.Error($"{nameof(Format)} is required");

            if (FormatArguments.Length == 0)
                return ValidationResult.Error($"{nameof(FormatArguments)} is required");

            return base.Validate();
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var args = settings.FormatArguments
            .Select(GetBestType)
            .ToArray();
        var text = string.Format(settings.Format, args);

        Console.Out.WriteLine(text);

        return 0;
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
