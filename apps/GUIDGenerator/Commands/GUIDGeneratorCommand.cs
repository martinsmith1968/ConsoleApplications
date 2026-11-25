using System.ComponentModel;
using System.Text;
using DNX.Helpers.Linq;
using DNX.Helpers.Strings;
using GUIDGenerator.Services;
using GUIDGenerator.Services.Converters;
using GUIDGenerator.Services.Converters.Interfaces;
using GUIDGenerator.Services.Generators;
using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace GUIDGenerator.Commands;

[Description("Generates one or more GUID values")]
public sealed class GUIDGeneratorCommand : Command<GUIDGeneratorCommand.Settings>
{
    public const string FORMAT_PLACEHOLDER_GUID_PREFIX = "{GUID";
    public const string FORMAT_PLACEHOLDER_INDEX_PREFIX = "{INDEX";

    public const string FORMAT_PLACEHOLDER_GUID = FORMAT_PLACEHOLDER_GUID_PREFIX + "}";
    public const string FORMAT_PLACEHOLDER_INDEX = FORMAT_PLACEHOLDER_INDEX_PREFIX + "}";

    public sealed class Settings : CommandSettings
    {
        [Description("How many GUID values to generate")]
        [CommandOption("-c|--Count")]
        [DefaultValue(1)]
        public int Count { get; set; }

        [Description("The text lines to use")]
        [CommandOption("-t|--Text")]
        public string[] Text { get; set; } = [];

        [Description("TODO")]
        [CommandOption("-m|--Mode")]
        [DefaultValue(GUIDGenerationModeType.Standard)]
        public GUIDGenerationModeType Mode { get; set; }

        [Description("TODO")]
        [CommandOption("-a|--CaseConversionType")]
        [DefaultValue(GUIDCaseConversionType.None)]
        public GUIDCaseConversionType CaseConversionType { get; set; }

        [Description("TODO")]
        [CommandOption("-f|--fmt|--Format")]
        public string? Format { get; set; }

        [Description("TODO")]
        [CommandOption("-d|--display|--DisplayText")]
        [DefaultValue(FORMAT_PLACEHOLDER_GUID)]
        public string? DisplayText { get; set; } = FORMAT_PLACEHOLDER_GUID;

        [Description("TODO")]
        [CommandOption("-e|--Delimiter")]
        public string Delimiter { get; set; } = Environment.NewLine;

        [Description("TODO")]
        [CommandOption("--ShowHeader")]
        [DefaultValue(true)]
        public bool ShowHeader { get; set; }

        [Description("TODO")]
        [CommandOption("-b|--copy|--CopyToClipboard")]
        [DefaultValue(false)]
        public bool CopyToClipboard { get; set; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var serviceProvider = BuildServiceProvider();

        var generatorFactory = serviceProvider.GetRequiredService<IGUIDGeneratorFactory>();

        var generator = generatorFactory.Create(settings.Mode);
        generator.Reset();

        var converter = serviceProvider.GetRequiredService<IGUIDConverter>();

        var iterator = settings.Text.HasAny()
            ? settings.Text.Select(t => converter.GenerateFrom(t))
            : Enumerable.Range(1, settings.Count).Select(i => generator.Generate());

        var outputWriter = Console.Out;

        var clipboardValue = new StringBuilder((40 + settings.Delimiter.Length) * iterator.Count());

        var index = 0;
        foreach (var guid in iterator)
        {
            ++index;

            var formattedValue = BuildDisplay(guid, index, settings.DisplayText, settings.Format, settings.CaseConversionType);

            if (settings.CopyToClipboard)
            {
                if (index > 1)
                    clipboardValue.Append(settings.Delimiter);
                clipboardValue.Append(formattedValue);
            }

            if (index > 1)
                outputWriter.Write(settings.Delimiter);
            outputWriter.Write(formattedValue);
        }

        outputWriter.WriteLine();

        return 0;
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var container = new ServiceCollection();

        container.AddTransient<IGUIDGenerator, StandardGUIDGenerator>();
        container.AddTransient<IGUIDGenerator, SequentialGUIDGenerator>();
        container.AddTransient<IGUIDGenerator, EntityFrameworkGUIDGenerator>();
        container.AddTransient<IGUIDGenerator, NHibernateGUIDCombGenerator>();

        container.AddTransient<IGUIDConverter, TextGUIDConverter>();

        container.AddTransient<IGUIDGeneratorFactory, GUIDGeneratorFactory>();

        return container.BuildServiceProvider();
    }

    private static string BuildDisplay(Guid guid, int index, string? displayText, string? format, GUIDCaseConversionType caseConversionType)
    {
        var guidValue = FormatGuid(guid, format);

        guidValue = ConvertCase(guidValue, caseConversionType);

        var textValue = ReplaceTextValues(displayText, index, guidValue);

        return textValue;
    }

    private static string ReplaceTextValues(string? displayText, int index, string guidValue)
    {
        displayText ??= string.Empty;

        if (!displayText.Contains(FORMAT_PLACEHOLDER_GUID_PREFIX))
            displayText += FORMAT_PLACEHOLDER_GUID;

        var stringFormat = displayText
                .Replace(FORMAT_PLACEHOLDER_INDEX_PREFIX, "{0")
                .Replace(FORMAT_PLACEHOLDER_GUID_PREFIX, "{1")
            ;

        var textValue = string.Format(stringFormat, index, guidValue);
        return textValue;
    }

    private static string FormatGuid(Guid guid, string? format)
    {
        return format.IsNullOrEmpty()
            ? guid.ToString()
            : guid.ToString(format);
    }

    private static string ConvertCase(string value, GUIDCaseConversionType postProcessing)
    {
        return postProcessing switch
        {
            GUIDCaseConversionType.Lower => value.ToLower(),
            GUIDCaseConversionType.Upper => value.ToUpper(),
            _ => value
        };
    }
}
