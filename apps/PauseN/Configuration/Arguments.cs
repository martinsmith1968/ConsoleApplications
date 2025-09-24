using DotMake.CommandLine;

// ReSharper disable InconsistentNaming

namespace PauseN.Configuration;

[CliCommand(Description = "Pause execution, optionally displaying a message")]
public class Arguments
{
    public const string PlaceHolder_TimeoutSeconds = "[#TimeoutSecondsParameter#]";

    [CliArgument(Description = "How long to wait (in seconds) before continuing", Order = 1, Arity = CliArgumentArity.ZeroOrOne, Required = false)]
    public int TimeoutSeconds { get; set; } = 10;

    [CliOption(Description = "How long to wait (in seconds) before continuing", Alias = "-t", Arity = CliArgumentArity.ZeroOrOne, Required = false)]
    public string Text { get; set; } = $"Press any key to continue (or wait {PlaceHolder_TimeoutSeconds} seconds)";

    [CliOption(Description = "The time to wait between checking for keypresses", Alias = "-s", Arity = CliArgumentArity.ZeroOrOne, Required = false)]
    public int SleepMilliseconds { get; set; }

    public void Validate()
    {
        if (TimeSpan.FromMilliseconds(SleepMilliseconds) > TimeSpan.FromSeconds(TimeoutSeconds))
            throw new Exception($"{nameof(SleepMilliseconds)} exceeds permitted value - is greater than {nameof(TimeoutSeconds)}");
    }

    public string DisplayText => Text
        .Replace(PlaceHolder_TimeoutSeconds, TimeoutSeconds.ToString())
        ;

    private static async Task Run(Arguments arguments)
    {
        await Console.Out.WriteAsync(arguments.DisplayText);

        var timeoutDate = DateTime.UtcNow.AddSeconds(arguments.TimeoutSeconds);

        while (DateTime.UtcNow < timeoutDate)
        {
            if (Console.KeyAvailable)
            {
                Console.ReadKey(true);
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        await Console.Out.WriteLineAsync();
    }
}
