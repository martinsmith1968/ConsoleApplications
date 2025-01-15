using Ookii.CommandLine;
using Toaster.Configuration;

namespace Toaster;

internal class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var arguments = new CommandLineParser<Arguments>()
                .Parse(args)
                ?? throw new Exception("Unable to parse command line");

            Console.WriteLine("Preparing...");

            // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
            //var contentBuilder = new ToastContentBuilder()
            //        .AddArgument("action", "viewConversation")
            //        .AddArgument("conversationId", 9813)
            //        .AddText("Andrew sent you a picture")
            //        .AddText("Check this out, The Enchantments in Washington!")
            //        .SetToastScenario(ToastScenario.Alarm)
            //    //.
            //    ;
            //
            //contentBuilder
            //    .Show(); // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater


            var contentBuilder = new ToastController()
                .Apply(arguments);

            contentBuilder.Show();



            Console.WriteLine("Done");
        }
        catch (Exception ex)
        {s
            Console.Error.WriteLine($"ERROR: {ex.Message}");
        }
    }
}
