using ConsoleApplications.Common.Configuration;
using Ookii.CommandLine.Commands;
using TomlFileHandler.Exceptions;

namespace TomlFileHandler;

internal class Program
{
    private const int DefaultErrorReturnCode = 999;
    private const int UnknownErrorReturnCode = 999;

    private static async Task<int> Main(string[] args)
    {
        try
        {
            var manager = new CommandManager(new CustomCommandOptions());

            var result = await manager.RunCommandAsync()
                         ?? UnknownErrorReturnCode;

            return result;
        }
        catch (ReturnCodeException ex)
        {
            await Console.Error.WriteLineAsync($"ERROR: {ex.Message}");
            return ex.ReturnCode;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"ERROR: {ex.Message}");

            return DefaultErrorReturnCode;
        }
    }
}
