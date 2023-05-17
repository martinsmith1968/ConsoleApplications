namespace ConsoleApplications.Common.Services;

public enum TimerState
{
    Initialised,
    Active,
    Paused,
    Stopped
}

public class NamedTimer
{
    private DateTime _startDateTime = DateTime.MinValue;

    public string Name { get; }

    public TimeSpan Elapsed => DateTime.UtcNow.Subtract(_startDateTime);

    public void Reset()
    {
        _startDateTime = DateTime.MinValue;
    }

    public NamedTimer(string name)
    {
        Name = name;
        Reset();
    }
}
