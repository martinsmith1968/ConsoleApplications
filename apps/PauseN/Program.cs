using DNX.Extensions.Assemblies;
using PauseN.Configuration;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;
using Spectre.Console.Rendering;

namespace PauseN;

internal class Program
{
    internal class CustomHelpProvider : HelpProvider
    {
        public CustomHelpProvider(ICommandAppSettings settings)
            : base(settings)
        {
        }

        public override IEnumerable<IRenderable> GetHeader(ICommandModel model, ICommandInfo? command)
        {
            var assemblyInfo = AssemblyDetails.ForEntryPoint();

            return new[]
            {
                new Text($"{assemblyInfo.Name} v{assemblyInfo.SimplifiedVersion} - {command.Description} ({assemblyInfo.Description})"), Text.NewLine,
                new Text($"Copyright © {assemblyInfo.Copyright}"), Text.NewLine,
                new Text("--------------------------------------"), Text.NewLine,
                Text.NewLine,
            };
        }
    }
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var app = new CommandApp<PauseNCommand>();
            app.Configure(config =>
            {
                config.SetApplicationName("PauseN");
                config.PropagateExceptions();
                config.ValidateExamples();
                config.UseAssemblyInformationalVersion();
                config.UseStrictParsing();
                config.CaseSensitivity(CaseSensitivity.All);
                config.SetHelpProvider(new CustomHelpProvider(config.Settings));
            });

            return await app.RunAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            return 1;
        }

        return 0;
    }
}
