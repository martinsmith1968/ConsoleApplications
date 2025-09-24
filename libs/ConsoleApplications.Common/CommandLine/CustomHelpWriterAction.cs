using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.Reflection;

namespace ConsoleApplications.Common.CommandLine;
public class CustomHelpWriterAction : SynchronousCommandLineAction
{
    private readonly HelpAction _defaultHelp;

    public CustomHelpWriterAction(HelpAction action) => _defaultHelp = action;

    public override int Invoke(System.CommandLine.ParseResult parseResult)
    {
        Console.Out.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name}");
        Console.Out.WriteLine($"Copyright");

        var result = _defaultHelp.Invoke(parseResult);

        return result;
    }
}
