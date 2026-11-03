using Xunit;

namespace TomlFileHandler.Tests.Data;

public static class TOMLFileServiceData
{
    public static TheoryData<string, string, string> KnownStringData()
    {
        return new TheoryData<string, string, string>()
        {
            { "database", "enabled", true.ToString() },
            { "database", "connection_max", "5000" },
            { "database", "server", "192.168.1.1" },
            { "database", "ports", "[ 8000, 8001, 8002 ]"},
            { "owner", "name", "Tom Preston-Werner" },
            { "owner", "age", "41.3" },
            { "", "title", "TOML Example" },
        };
    }

    public static TheoryData<string, string, int, string> KnownStringArrayData()
    {
        return new TheoryData<string, string, int, string>()
        {
            { "database", "ports", 1, "8001"},
        };
    }

    public static TheoryData<string, string, int> InvalidStringArrayData()
    {
        return new TheoryData<string, string, int>()
        {
            { "database", "server", 1 },
        };
    }

    public static TheoryData<string, string, int> KnownIntData()
    {
        return new TheoryData<string, string, int>()
        {
            { "database", "connection_max", 5000 },
        };
    }

    public static TheoryData<string, string, int, int> KnownIntArrayData()
    {
        return new TheoryData<string, string, int, int>()
        {
            { "database", "ports", 1, 8001 },
        };
    }

    public static TheoryData<string, string, long> KnownLongData()
    {
        return new TheoryData<string, string, long>()
        {
            { "database", "connection_max", 5000 },
        };
    }

    public static TheoryData<string, string, int, long> KnownLongArrayData()
    {
        return new TheoryData<string, string, int, long>()
        {
            { "database", "ports", 0, 8000 },
            { "database", "ports", 1, 8001 },
            { "database", "ports", 2, 8002 },
            { "database", "ports", -1, 8002 },
        };
    }

    public static TheoryData<string, string, bool> KnownBoolData()
    {
        return new TheoryData<string, string, bool>()
        {
            { "database", "enabled", true },
        };
    }

    public static TheoryData<string, string, double> KnownFloatData()
    {
        return new TheoryData<string, string, double>()
        {
            { "owner", "age", 41.3 },
        };
    }

    public static TheoryData<string, string, DateTime> KnownDateTimeData()
    {
        return new TheoryData<string, string, DateTime>()
        {
            { "owner", "dob2", new DateTime(1979, 05, 27, 07, 32, 00, DateTimeKind.Local) },
        };
    }

    public static TheoryData<string, string, DateTimeOffset> KnownDateTimeOffsetData()
    {
        return new TheoryData<string, string, DateTimeOffset>()
        {
            { "owner", "dob", new DateTimeOffset(1979, 05, 27, 07, 32, 00, TimeSpan.FromHours(-08)) },
        };
    }
}
