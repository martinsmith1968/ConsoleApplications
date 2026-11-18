using Calendar.Commands;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Extensions;
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
                    config.AddCustomCommandWithExtensions<MonthCalendarCommand>();
                    config.AddCustomCommandWithExtensions<YearCalendarCommand>();

                    CommonCommandApp.Configure(config);
                }
            );

            return await app.RunAsync(args);
        }
        catch (Exception e)
        {
            CommonCommandApp.DisplayError(e.Message);
            return 1;
        }
    }
}
