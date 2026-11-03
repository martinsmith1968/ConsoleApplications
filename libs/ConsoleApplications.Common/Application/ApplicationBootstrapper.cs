using Microsoft.Extensions.DependencyInjection;
using Ookii.CommandLine;

namespace ConsoleApplications.Common.Application;
public class ApplicationBootstrapper
{
    public static readonly ParseOptions DefaultParseOptions = new()
    {
        AutoVersionArgument = true
    };

    /// <summary>
    /// Bootstraps the application with target methods and processing
    /// </summary>
    /// <typeparam name="T">The Arguments type to parse the command line against</typeparam>
    /// <param name="args">The arguments supplied.</param>
    /// <param name="handlerAsync">The handler asynchronous.</param>
    /// <param name="parserOptions">The parser options.</param>
    /// <param name="serviceProviderBuilder">The service provider builder.</param>
    /// <param name="defaultArgumentFailureExitCode">The default argument failure exit code.</param>
    /// <param name="defaultErrorExitCode">The default error exit code.</param>
    /// <returns>
    ///   integer return code
    /// </returns>
    /// <exception cref="System.Exception">Unable to Parse Command Line</exception>
    public static async Task<int> ExecuteAsync<T>(
        string[] args,
        Func<T, IServiceProvider, Task> handlerAsync,
        ParseOptions parserOptions = null,
        Func<IServiceProvider> serviceProviderBuilder = null,
        int defaultArgumentFailureExitCode = 1,
        int defaultErrorExitCode = 2
    )
        where T : class
    {
        parserOptions ??= DefaultParseOptions;

        try
        {
            var serviceProvider = serviceProviderBuilder?.Invoke()
                                  ?? new ServiceCollection().BuildServiceProvider();

            var arguments = CommandLineParser.Parse<T>(args, parserOptions)
                            ?? throw new Exception("Unable to Parse Command Line");
            if (arguments is IValidatableArguments validatableArguments)
                validatableArguments.Validate();

            await handlerAsync(arguments, serviceProvider);
        }
        catch (CommandLineArgumentException e)
        {
            // TODO: Might be parserOptions dependant as to whether this shows automatically
            await Console.Error.WriteLineAsync($"ERROR: {e.Message}");
            return defaultArgumentFailureExitCode;
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"ERROR: {e.Message}");
            return defaultErrorExitCode;
        }

        return 0;
    }
}
