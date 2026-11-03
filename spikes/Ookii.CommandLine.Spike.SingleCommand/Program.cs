using Ookii.CommandLine.Spike.SingleCommand.Configuration;

namespace Ookii.CommandLine.Spike.SingleCommand;

// See : https://www.ookii.org/Software/CommandLineParser/

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var options = new ParseOptions()
            {
                ArgumentNamePrefixes = [ "-" ],
                AllowWhiteSpaceValueSeparator = true,
                AutoHelpArgument = true,
                AutoVersionArgument = true,
                Mode = ParsingMode.LongShort,
                UsageWriter = new MyUsageWriter()
            };

            var arguments = Arguments.Parse(options);
            if (arguments == null)
                throw new ArgumentNullException(nameof(arguments));

            foreach (var name in arguments.Name)
            {
                await Console.Out.WriteLineAsync($"Name: {name}, BirthDate: {arguments.BirthDate:o}, SwitchOn: {arguments.SwitchOn}, SwitchOff: {arguments.SwitchOff}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"ERROR: {ex.Message}");
            return 1;
        }
    }
}
