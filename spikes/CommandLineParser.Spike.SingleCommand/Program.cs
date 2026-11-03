using CommandLine;
using CommandLineParser.Spike.SingleCommand.Configuration;

namespace CommandLineParser.Spike.SingleCommand;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        try
        {
            var parser = new Parser(s =>
            {
                s.AutoHelp = true;
                s.AutoVersion = true;
                s.IgnoreUnknownArguments = true;
                s.HelpWriter = Console.Out;
            });

            parser.ParseArguments<Arguments>(args)
                .WithParsed(o =>
                {
                    foreach(var name in o.Name)
                    {
                        Console.Out.WriteLine($"Name: {name}, BirthDate: {o.BirthDate:o}, SwitchOn: {o.SwitchOn}, SwitchOff: {o.SwitchOff}");
                    }
                })
                .WithNotParsed(e =>
                {
                    foreach (var x in e)
                    {
                        Console.Error.WriteLine(x.ToString());
                    }
                });

            return 0;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"ERROR: {ex}");
            return 1;
        }
    }
}
