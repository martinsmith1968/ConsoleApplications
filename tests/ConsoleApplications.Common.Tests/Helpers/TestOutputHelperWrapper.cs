using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace ConsoleApplications.Common.Tests.Helpers;

public class TestOutputHelperWrapper(ITestOutputHelper output) : TextWriter
{
    public override void WriteLine(string message)
    {
        try
        {
            output?.WriteLine($"DEBUG: {message}");

        }
        catch { }
    }

    public override void WriteLine(string format, params object?[] args)
    {
        output?.WriteLine("DEBUG:" + format, args);
    }

    public override Encoding Encoding { get; } = Encoding.UTF8;
}

public static class TestOutputHelperWrapperExtensions
{
    public static void AttachTraceListeners(this ITestOutputHelper outputHelper)
    {
        Trace.Listeners.Add(new TextWriterTraceListener(new TestOutputHelperWrapper(outputHelper)));
    }
}
