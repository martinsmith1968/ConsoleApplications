using Ookii.CommandLine;
using TomlFileHandler.Configuration;
using Tommy;

#pragma warning disable CA2208

namespace TomlFileHandler;

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
        using var fileReader = File.OpenText(arguments.FileInfo.FullName);
        var table = TOML.Parse(fileReader);

        return arguments.HasSectionName
            ? table[arguments.SectionName][arguments.KeyName]
            : table[arguments.KeyName];
    }

    private static string WriteValue(Arguments arguments)
    {
        using var fileReader = File.OpenText(arguments.FileInfo.FullName);
        var table = TOML.Parse(fileReader);

        if (arguments.HasSectionName)
            table[arguments.SectionName][arguments.KeyName] = arguments.Value;
        else
            table[arguments.KeyName] = arguments.Value;

        return string.Empty;
    }
}
