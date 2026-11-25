using System.ComponentModel;
using IniParser;
using Spectre.Console;
using Spectre.Console.Cli;

namespace IniFileHandler.Commands;

public enum CommandType
{
    Read,
    Write
}

[Description("Reads or writes a key's value in a section of an INI file")]
public sealed class IniFileHandlerCommand : Command<IniFileHandlerCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("The Command to execute")]
        [CommandArgument(0, "<command>")]
        public CommandType Command { get; set; }

        [Description("The file name to access")]
        [CommandOption("-f|--FileName")]
        public string FileName { get; set; } = string.Empty;

        [Description("The Section name to access")]
        [CommandOption("-s|--SectionName")]
        public string SectionName { get; set; } = string.Empty;

        [Description("The Key name to access")]
        [CommandOption("-k|--KeyName")]
        public string KeyName { get; set; } = string.Empty;

        [Description("The Value to get / set")]
        [CommandOption("-v|--Value")]
        public string Value { get; set; } = string.Empty;

        public FileInfo? FileInfo
        {
            get
            {
                try
                {
                    return new FileInfo(FileName);
                }
                catch
                {
                    return null;
                }
            }
        }

        public override ValidationResult Validate()
        {
            if (FileInfo == null)
                return ValidationResult.Error($"Invalid {nameof(FileName)}: {FileName}");

            if (Command == CommandType.Write && string.IsNullOrEmpty(Value))
                return ValidationResult.Error($"{nameof(Value)} must to be specified for command: {Command}");

            return base.Validate();
        }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var result = settings.Command switch
        {
            CommandType.Read => ReadValue(settings),
            CommandType.Write => WriteValue(settings),
            _ => throw new ArgumentException($"Invalid or unsupported command: {settings.Command}", nameof(Settings.Command))
        };

        if (!string.IsNullOrEmpty(result))
            Console.Out.WriteLine(result);

        return 0;
    }

    private static string ReadValue(Settings settings)
    {
        var iniFile = new FileIniDataParser().ReadFile(settings.FileInfo?.FullName);

        return iniFile[settings.SectionName][settings.KeyName];
    }

    private static string WriteValue(Settings settings)
    {
        var iniFile = new FileIniDataParser().ReadFile(settings.FileInfo?.FullName);

        iniFile[settings.SectionName][settings.KeyName] = settings.Value;

        return string.Empty;
    }
}
