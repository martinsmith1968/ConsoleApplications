namespace ConsoleApplications.Common.Interfaces;

public interface ICustomCommand
{
    public abstract static string GetCommandName();

    public virtual static IList<string[]> GetExamples() => [ ];

    public virtual static string[] GetNotes() => [ ];
}
