namespace ConsoleApplications.Common.Interfaces;

public interface ICustomParseOptionsOverride
{
    bool UseDefaultArgumentsFile { get; set; }

    bool UseLocalArgumentsFile { get; set; }
}
