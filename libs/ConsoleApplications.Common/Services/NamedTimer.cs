namespace ConsoleApplications.Common.Services;

public enum TimerState
{
    Initialised,
    Active,
    Paused,
    Stopped
}

public class NamedTimer(string name)
{
    private DateTime _startDateTime = DateTime.MinValue;

    public string Name { get; } = name;
    public DateTime StartDateTime { get; private set; } = DateTime.MinValue;
    public TimerState State { get; private set; } = TimerState.Initialised;

    private TimeSpan _accumulatedTime = TimeSpan.Zero;
    public TimeSpan Elapsed => State switch
    {
        TimerState.Initialised => TimeSpan.Zero,
        TimerState.Paused      => _accumulatedTime,
        TimerState.Stopped     => _accumulatedTime,
        _                      => _accumulatedTime + DateTime.UtcNow.Subtract(_startDateTime),
    };

    public void Reset()
    {
        _accumulatedTime = TimeSpan.Zero;
        _startDateTime = DateTime.MinValue;
    }

    public void Start()
    {
        StartDateTime = DateTime.UtcNow;
        State = TimerState.Active;
    }

    public void Stop()
    {
        Pause();
        State = TimerState.Stopped;
    }

    public void Pause()
    {
        _accumulatedTime += Elapsed;
        State = TimerState.Paused;
    }

    public void Resume()
    {
        if (State == TimerState.Paused)
        {
            Start();
        } else if (State == TimerState.Stopped)
        {
            Reset();
            Start();
        }
    }
}
