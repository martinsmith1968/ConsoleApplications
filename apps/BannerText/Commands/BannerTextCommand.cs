using System.ComponentModel;
using BannerText.Configuration;
using BannerText.Services;
using ConsoleApplications.Common.CommandLine;
using ConsoleApplications.Common.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BannerText.Commands;

[Description("Prints text banners, with alignment and header/footer/affix options")]
public sealed class BannerTextCommand : Command<BannerTextCommand.Settings>, ICustomCommand
{
    public static string GetCommandName() => CommonCommandApp.DeriveCommandName<BannerTextCommand>();

    public sealed class Settings : CommandSettings
    {
        private const int HeaderFooterLineCountLimit = 100;
        private const int AffixSizeLimit = 1000;
        private const int TotalLengthLimit = 10000;

        [Description("The text line(s) to display")]
        [CommandArgument(0, "<text>")]
        public string[] Text { get; set; } = [];

        [Description("The character to use for header lines")]
        [CommandOption($"--hlc|--{nameof(HeaderLineChar)}")]
        [DefaultValue('*')]
        public char HeaderLineChar { get; set; }

        [Description("The number of header lines to print")]
        [CommandOption($"--hln|--{nameof(HeaderLineCount)}")]
        [DefaultValue(1)]
        public int HeaderLineCount { get; set; }

        [Description("The character to use for footer lines")]
        [CommandOption($"--flc|--{nameof(FooterLineChar)}")]
        [DefaultValue('*')]
        public char FooterLineChar { get; set; }

        [Description("The number of footer lines to print")]
        [CommandOption($"--fln|--{nameof(FooterLineCount)}")]
        [DefaultValue(1)]
        public int FooterLineCount { get; set; }

        [Description("The character to use for the text line prefix/suffix")]
        [CommandOption($"--tlc|--{nameof(TextLineChar)}")]
        [DefaultValue('*')]
        public char TextLineChar { get; set; }

        [Description("The number of prefix characters before each text line")]
        [CommandOption($"--tpc|--{nameof(TitlePrefixCount)}")]
        [DefaultValue(2)]
        public int TitlePrefixCount { get; set; }

        [Description("The number of suffix characters after each text line")]
        [CommandOption($"--tsc|--{nameof(TitleSuffixCount)}")]
        [DefaultValue(2)]
        public int TitleSuffixCount { get; set; }

        [Description("The gap size between the prefix and the text")]
        [CommandOption($"--tpgs|--{nameof(TitlePrefixGapSize)}")]
        [DefaultValue(2)]
        public int TitlePrefixGapSize { get; set; }

        [Description("The gap size between the text and the suffix")]
        [CommandOption($"--tsgs|--{nameof(TitleSuffixGapSize)}")]
        [DefaultValue(2)]
        public int TitleSuffixGapSize { get; set; }

        [Description("How to align the text line(s)")]
        [CommandOption($"--ta|--{nameof(Alignment)}")]
        [DefaultValue(TextAlignmentType.Left)]
        public TextAlignmentType Alignment { get; set; }

        [Description("The minimum total line length")]
        [CommandOption($"--minl|--{nameof(MinTotalLength)}")]
        [DefaultValue(0)]
        public int MinTotalLength { get; set; }

        [Description("The maximum total line length")]
        [CommandOption($"--maxl|--{nameof(MaxTotalLength)}")]
        [DefaultValue(0)]
        public int MaxTotalLength { get; set; }

        public override ValidationResult Validate()
        {
            if (HeaderLineCount is < 0 or > HeaderFooterLineCountLimit)
                return ValidationResult.Error($"{nameof(HeaderLineCount)} must be between 0 and {HeaderFooterLineCountLimit}");
            if (FooterLineCount is < 0 or > HeaderFooterLineCountLimit)
                return ValidationResult.Error($"{nameof(FooterLineCount)} must be between 0 and {HeaderFooterLineCountLimit}");

            if (TitlePrefixCount is < 0 or > AffixSizeLimit)
                return ValidationResult.Error($"{nameof(TitlePrefixCount)} must be between 0 and {AffixSizeLimit}");
            if (TitleSuffixCount is < 0 or > AffixSizeLimit)
                return ValidationResult.Error($"{nameof(TitleSuffixCount)} must be between 0 and {AffixSizeLimit}");
            if (TitlePrefixGapSize is < 0 or > AffixSizeLimit)
                return ValidationResult.Error($"{nameof(TitlePrefixGapSize)} must be between 0 and {AffixSizeLimit}");
            if (TitleSuffixGapSize is < 0 or > AffixSizeLimit)
                return ValidationResult.Error($"{nameof(TitleSuffixGapSize)} must be between 0 and {AffixSizeLimit}");

            if (MinTotalLength is < 0 or > TotalLengthLimit)
                return ValidationResult.Error($"{nameof(MinTotalLength)} must be between 0 and {TotalLengthLimit}");
            if (MaxTotalLength is < 0 or > TotalLengthLimit)
                return ValidationResult.Error($"{nameof(MaxTotalLength)} must be between 0 and {TotalLengthLimit}");

            if (MaxTotalLength > 0 && MinTotalLength > MaxTotalLength)
                return ValidationResult.Error($"{nameof(MinTotalLength)} ({MinTotalLength}) must not be greater than {nameof(MaxTotalLength)} ({MaxTotalLength})");

            return base.Validate();
        }

        public BannerOptions ToBannerOptions() => new()
        {
            HeaderLineChar = HeaderLineChar,
            HeaderLineCount = HeaderLineCount,
            FooterLineChar = FooterLineChar,
            FooterLineCount = FooterLineCount,
            TextLineChar = TextLineChar,
            TitlePrefixCount = TitlePrefixCount,
            TitleSuffixCount = TitleSuffixCount,
            TitlePrefixGapSize = TitlePrefixGapSize,
            TitleSuffixGapSize = TitleSuffixGapSize,
            Alignment = Alignment,
            MinTotalLength = MinTotalLength,
            MaxTotalLength = MaxTotalLength,
        };
    }

    public static IList<string[]> GetExamples()
    {
        return new List<string[]>
        {
            { [ "\"Hello World\"" ] },
            { [ "\"Hello World\"", "--minl", "80" ] },
            { [ "\"Hello World\"", "--minl", "80", "--ta", "Center" ] },
            { [ "\"Hello World\"", "--hlc", "#", "--flc", "#", "--tlc", "#" ] },
        };
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var lines = BannerRenderer.Generate(settings.Text, settings.ToBannerOptions());

        foreach (var line in lines)
        {
            Console.Out.WriteLine(line);
        }

        return 0;
    }
}
