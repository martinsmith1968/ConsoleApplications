using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

// ReSharper disable InconsistentNaming

namespace PauseN.Configuration;


public sealed class PauseNCommand : Command<PauseNCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSeconds#]";

        [Description("How long to wait (in seconds) before continuing")]
        [CommandArgument(0, "[seconds]")]
        [DefaultValue(10)]
        public int TimeoutSeconds { get; set; }

        [Description("The text to display")]
        [CommandOption("-t|--text", isRequired: false)]
        [DefaultValue($"Press any key to continue (or wait {PlaceHolder_TimeoutSeconds} seconds)")]
        public string Text { get; set; } = "";

        [Description("How long to wait (in seconds) before continuing")]
        [CommandOption("-s|--sleep", isRequired: false)]
        [DefaultValue(100)]
        public int SleepMilliseconds { get; set; }

        public override ValidationResult Validate()
        {
            if (TimeSpan.FromMilliseconds(SleepMilliseconds) > TimeSpan.FromSeconds(TimeoutSeconds))
                return ValidationResult.Error($"{nameof(SleepMilliseconds)} exceeds permitted value - is greater than {nameof(TimeoutSeconds)}");

            return ValidationResult.Success();
        }

        public string DisplayText => Text
            .Replace(PlaceHolder_TimeoutSeconds, TimeoutSeconds.ToString());
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        Console.Out.Write(settings.DisplayText);

        var timeoutDate = DateTime.UtcNow.AddSeconds(settings.TimeoutSeconds);

        while (DateTime.UtcNow < timeoutDate)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        Console.Out.WriteLine();

        return 0;
    }
}
