using GUIDGenerator.Services.Generators.Interfaces;
using GUIDGenerator.Services.Types;

namespace GUIDGenerator.Services.Generators;

public class NHibernateGUIDCombGenerator : IGUIDGenerator
{
    private static readonly DateTime BaseDate = new(1900, 1, 1);

    public GUIDGenerationModeType Type => GUIDGenerationModeType.NHibernate;

    public void Reset()
    {
    }

    /// <remarks>
    /// From : https://stackoverflow.com/questions/1752004/sequential-guid-generator/42450159#42450159
    /// </remarks>
    public Guid Generate()
    {
        var guidArray = Guid.NewGuid().ToByteArray();

        var now = DateTime.UtcNow;

        // Get the days and milliseconds which will be used to build the byte string
        var days = new TimeSpan(now.Ticks - BaseDate.Ticks);
        var milliseconds = now.TimeOfDay;

        // Convert to a byte array
        // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
        var daysArray = BitConverter.GetBytes(days.Days);
        var millisecondsArray = BitConverter.GetBytes((long)(milliseconds.TotalMilliseconds / 3.333333));

        // Reverse the bytes to match SQL Servers ordering
        Array.Reverse(daysArray);
        Array.Reverse(millisecondsArray);

        // Copy the bytes into the guid
        Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
        Array.Copy(millisecondsArray, millisecondsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

        return new Guid(guidArray);
    }
}
