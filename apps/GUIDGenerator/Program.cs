using System.Text;
using ConsoleApplications.Common.Application;
using DNX.Helpers.Linq;
using DNX.Helpers.Strings;
using GUIDGenerator.Configuration;
using GUIDGenerator.Services;
using GUIDGenerator.Services.Converters;
using GUIDGenerator.Services.Converters.Interfaces;
using GUIDGenerator.Services.Generators;
using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;
using Microsoft.Extensions.DependencyInjection;

namespace GUIDGenerator;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        return await ApplicationBootstrapper.ExecuteAsync<Arguments>(
            args,
            ProcessAsync,
            serviceProviderBuilder: ServiceProviderBuilder,
            parserOptions: Arguments.Options
        );
    }

    private static IServiceProvider ServiceProviderBuilder()
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

    private static async Task ProcessAsync(Arguments arguments, IServiceProvider serviceProvider)
    {
        var generatorFactory = serviceProvider.GetRequiredService<IGUIDGeneratorFactory>();

        var generator = generatorFactory.Create(arguments.Mode);
        generator.Reset();

        var converter = serviceProvider.GetRequiredService<IGUIDConverter>();

        var iterator = arguments.Text.HasAny()
            ? arguments.Text.Select(t => converter.GenerateFrom(t))
            : Enumerable.Range(1, arguments.Count).Select(i => generator.Generate());

        // TODO: Allow output to file ?
        TextWriter outputWriter = Console.Out;

        var clipboardValue = new StringBuilder((40 + arguments.Delimiter.Length) * iterator.Count());

        var index = 0;
        foreach (var guid in iterator)
        {
            ++index;

            var formattedValue = BuildDisplay(guid, index, arguments.DisplayText, arguments.Format, arguments.CaseConversionType);

            if (arguments.CopyToClipboard)
            {
                if (index > 1)
                    clipboardValue.Append(arguments.Delimiter);
                clipboardValue.Append(formattedValue);
            }

            if (index > 1)
                await outputWriter.WriteAsync(arguments.Delimiter);
            await outputWriter.WriteAsync(formattedValue);
        }

        await outputWriter.WriteLineAsync();
    }

    private static string BuildDisplay(Guid guid, int index, string displayText, string format, GUIDCaseConversionType caseConversionType)
    {
        var guidValue = FormatGuid(guid, format);

        guidValue = ConvertCase(guidValue, caseConversionType);

        var textValue = ReplaceTextValues(displayText, index, guidValue);

        return textValue;
    }

    private static string ReplaceTextValues(string displayText, int index, string guidValue)
    {
        if (!displayText.Contains(Arguments.FORMAT_PLACEHOLDER_GUID_PREFIX))
            displayText += Arguments.FORMAT_PLACEHOLDER_GUID;

        var stringFormat = displayText
                .Replace(Arguments.FORMAT_PLACEHOLDER_INDEX_PREFIX, "{0")
                .Replace(Arguments.FORMAT_PLACEHOLDER_GUID_PREFIX, "{1")
            ;

        var textValue = string.Format(stringFormat, index, guidValue);
        return textValue;
    }

    private static string FormatGuid(Guid guid, string format)
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
