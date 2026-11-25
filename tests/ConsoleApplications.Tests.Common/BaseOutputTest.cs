using System.Text;

namespace ConsoleApplications.Tests.Common;

public abstract class BaseOutputTest
{
    protected readonly StringBuilder OutputText = new();
    protected readonly StringWriter OutputWriter;
    protected readonly StringBuilder ErrorText = new();
    protected readonly StringWriter ErrorWriter;

    protected BaseOutputTest()
    {
        OutputWriter = new StringWriter(OutputText);
        ErrorWriter = new StringWriter(ErrorText);

        Console.SetOut(OutputWriter);
        Console.SetError(ErrorWriter);
    }
}
