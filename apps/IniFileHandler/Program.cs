using IniFileHandler.Configuration;
using IniParser;
using Ookii.CommandLine;

#pragma warning disable CA2208

namespace IniFileHandler;

internal class Program
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
        var result = arguments.Command switch
        {
            CommandType.Read => ReadValue(arguments),
            CommandType.Write => WriteValue(arguments),
            _ => throw new ArgumentException($"Invalid or unsupported command: {arguments.Command}", nameof(Arguments.Command))
        };

        if (!string.IsNullOrEmpty(result))
            await Console.Out.WriteLineAsync(result);
    }

    private static string ReadValue(Arguments arguments)
    {
        var iniFile = new FileIniDataParser().ReadFile(arguments.FileInfo.FullName);

        return iniFile[arguments.SectionName][arguments.KeyName];
    }

    private static string WriteValue(Arguments arguments)
    {
        var iniFile = new FileIniDataParser().ReadFile(arguments.FileInfo.FullName);

        iniFile[arguments.SectionName][arguments.KeyName] = arguments.Value;

        return string.Empty;
    }
}
