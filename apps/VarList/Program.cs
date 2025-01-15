namespace VarList;

public class Program
{
    public static void Main(string[] args)
    {
    }
}

/*
using CommandLine;
using CommandLine.Text;
using VarList.Commands;

namespace VarList;

internal class Program
{
    static void Main(string[] args)
    {
        var app = new CommandApp<VarListCommand>();

        app.Configure(
            x =>
            {
                x.PropagateExceptions();
            });


        try
        {
            app.Run(args);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]ERROR:[/] {ex.Message}");
            //AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        }
    }



    static void Main(string[] args)
    {
        var result = CustomParser.Parse<VarListCommand>(args)
            .WithParsed(x => x.Execute())
            .WithNotParsed(errs => CustomParser.DisplayHelpText(errs));
    }


}

public class CustomParser
{
    public static Parser CreateCustomParser()
    {
        var parser = new Parser(
            x =>
            {
                x.HelpWriter = null;
                x.AutoHelp = false;
                x.AutoVersion = false;
                x.IgnoreUnknownArguments = false;
                x.CaseSensitive = true;
                x.CaseInsensitiveEnumValues = true;
            }
        );

        return parser;
    }

    public static ParserResult<T> Parse<T>(IEnumerable<string> args)
    {
        var parser = CreateCustomParser();

        var result = parser.ParseArguments<T>(args);

        return result;
    }

    public static void ParseAndRun<T>(IEnumerable<string> args, Action<T> parsed, Action<ParserResult<T>, IEnumerable<Error>> notParsed)
    {
        var parser = CreateCustomParser();

        var result = parser.ParseArguments<T>(args);

        result
            .WithParsed(x => parsed(x))
            .WithNotParsed(x => notParsed(result, x));
    }

    public static void DisplayHelpText<T>(ParserResult<T> result, IEnumerable<Error> errors, TextWriter outputWriter = null)
    {
        var helpText = errors.IsVersion()
            ? HelpText.AutoBuild(result, 80)
            : HelpText.AutoBuild(
                result,
                x =>
                {
                    x.Copyright = "Bob";
                    x.AdditionalNewLineAfterOption = false;
                    return HelpText.DefaultParsingErrorsHandler(result, x);
                }
            );

        (outputWriter ?? Console.Error).WriteLine(helpText);
    }
}
*/
