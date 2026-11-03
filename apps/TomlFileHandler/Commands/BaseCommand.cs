using ConsoleApplications.Common.Application;
using Ookii.CommandLine.Commands;

namespace TomlFileHandler.Commands;

public abstract class BaseCommand : ICommand, IValidatableArguments
{
    public int Run()
    {
        Execute();

        return 0;
    }

    public abstract void Execute();
    public virtual void Validate()
    {
    }
}
