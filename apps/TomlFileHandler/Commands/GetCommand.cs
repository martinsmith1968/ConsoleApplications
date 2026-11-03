using System.ComponentModel;
using Ookii.CommandLine;
using Ookii.CommandLine.Commands;
using TomlFileHandler.Exceptions;
using TomlFileHandler.Services;
using TomlFileHandler.Types;
using Tommy;

namespace TomlFileHandler.Commands;

[GeneratedParser]
[Command("GET")]
[Description("Get a value from a TOML file")]
internal partial class GetCommand : BaseTOMLValueCommand
{
    [CommandLineArgument(IsRequired = false)]
    [Description("The specific Data Type to expect (Default: String)")]
    [Alias("type")]
    public TOMLDataType DataType { get; set; } = TOMLDataType.String;

    [CommandLineArgument(IsRequired = false)]
    [Description("How to format the value before output")]
    [Alias("fmt")]
    public string Format { get; set; } = null!;

    public override void Execute()
    {
        var table = TommyTOMLFileService.ReadFile(FileNameInfo.FullName);

        var value = ConvertAndFormatNodeValue(table);

        Console.Out.WriteLine(value);
    }

    private string ConvertAndFormatNodeValue(TomlTable table)
    {
        var node = TommyTOMLFileService.GetTomlNode(table, SectionName, KeyName, ValueIndex);
        if (node == null)
            throw new ReturnCodeException(1, $"Key: {KeyName} not found");

        object value = DataType switch
        {
            TOMLDataType.Int => TommyTOMLFileService.GetTomlValueAsInt(table, SectionName, KeyName, ValueIndex),
            TOMLDataType.Long => TommyTOMLFileService.GetTomlValueAsLong(table, SectionName, KeyName, ValueIndex),
            TOMLDataType.Bool => TommyTOMLFileService.GetTomlValueAsBool(table, SectionName, KeyName, ValueIndex),
            TOMLDataType.Float => TommyTOMLFileService.GetTomlValueAsFloat(table, SectionName, KeyName, ValueIndex),
            TOMLDataType.DateTime => TommyTOMLFileService.GetTomlValueAsDateTime(table, SectionName, KeyName, ValueIndex),
            TOMLDataType.DateTimeOffset => TommyTOMLFileService.GetTomlValueAsDateTimeOffset(table, SectionName, KeyName, ValueIndex),
            _ => TommyTOMLFileService.GetTomlValue(table, SectionName, KeyName, ValueIndex)
        };

        var result = string.IsNullOrWhiteSpace(Format)
            ? value?.ToString()
            : string.Format("{0:" + Format + "}", value);

        return result;
    }
}
