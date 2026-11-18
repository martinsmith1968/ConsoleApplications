using ConsoleApplications.Common.Interfaces;
using DNX.Extensions.Strings;
using Spectre.Console.Cli;

namespace ConsoleApplications.Common.Extensions;

public static class ConfiguratorExtensions
{
    public static ICommandConfigurator AddCustomCommandWithExtensions<T>(this IConfigurator configurator)
        where T : class, ICommand, ICustomCommand
    {
        var name = T.GetCommandName()
            .CoalesceNullOrEmptyWith(typeof(T).Name);

        var commandConfigurator = configurator.AddCommand<T>(name);

        var examples = T.GetExamples();
        foreach (var example in examples)
        {
            configurator.AddExample(example);
        }

        return commandConfigurator;
    }
}
