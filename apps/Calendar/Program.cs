using Calendar.Commands;
using ConsoleApplications.Common.CommandLine;
using Spectre.Console.Cli;

namespace Calendar;

internal class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.AddCommand<MonthCalendarCommand>(MonthCalendarCommand.CommandName);
                config.AddCommand<YearCalendarCommand>(YearCalendarCommand.CommandName);

                CustomCommandAppConfiguration.Configure(config);

                //config.AddExample("");
                //config.AddExample("-y", "2000");
                //config.AddExample("-m", "7");
                //config.AddExample("-y", "2000", "-m", "7");
            });

            return await app.RunAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }
    }
}
