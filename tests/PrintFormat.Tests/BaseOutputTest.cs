using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using DNX.Extensions.Assemblies;

namespace PrintFormat.Tests;

public class BaseOutputTest
{
    protected readonly StringBuilder OutputText = new StringBuilder();
    protected readonly StringWriter OutputWriter;
    protected readonly StringBuilder ErrorText = new StringBuilder();
    protected readonly StringWriter ErrorWriter;

    public BaseOutputTest()
    {
        OutputWriter = new StringWriter(OutputText);
        ErrorWriter = new StringWriter(ErrorText);

        Console.SetOut(OutputWriter);
        Console.SetError(ErrorWriter);
    }
    protected static string GetExpectedOutputFileName([CallerFilePath] string className = "", [CallerMemberName] string methodName = "")
    {
        var relativeResourceName = $"ExpectedOutput.{Path.GetFileNameWithoutExtension(className)}.{methodName}.txt";

        return Assembly.GetExecutingAssembly().GetEmbeddedResourceText(relativeResourceName);
    }

    protected void AssertExpectedOutput([CallerFilePath] string className = "", [CallerMemberName] string methodName = "")
    {
        var expectedOutput = GetExpectedOutputFileName(className, methodName);

        OutputText.ToString().ShouldBe(expectedOutput);
    }
}
