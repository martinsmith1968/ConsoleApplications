using Tomlyn;
using Tomlyn.Model;

namespace TomlFileHandler.Services;

public class TomlynTOMLFileService
{
    public static TomlTable ReadFile(string fileName)
    {
        var text = File.ReadAllText(fileName);

        return ReadText(text);
    }

    public static TomlTable? ReadText(string text)
    {
        return TomlSerializer.Deserialize<TomlTable>(text);
    }


    /// <summary>
    /// Unlike Tommy's TomlNode, Tomlyn stores scalar values as plain boxed CLR types (long/double/bool/string)
    /// and dates as <see cref="TomlDateTime"/> - only tables/arrays are TomlObject. So this returns the raw
    /// value (or, for a whole array with no index, a "[ a, b, c ]" string, since TomlArray doesn't format itself).
    /// </summary>
    public static object? GetTomlNode(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var container = !string.IsNullOrWhiteSpace(sectionName)
            ? TryGet(table, sectionName, out var sectionValue) ? sectionValue as TomlTable : null
            : table;

        var node = container is not null && TryGet(container, keyName, out var keyValue) ? keyValue : null;

        if (valueIndex.HasValue)
        {
            if (node is not TomlArray valueArray)
            {
                throw new ArgumentOutOfRangeException(keyName, valueIndex, $"Cannot access Index: {valueIndex} - '{keyName}' is not an array");
            }

            var arrayIndex = valueIndex.Value >= 0
                ? valueIndex.Value
                : valueArray.Count + valueIndex.Value;

            return valueArray[arrayIndex];
        }

        return node is TomlArray array ? FormatArray(array) : node;
    }

    private static string FormatArray(TomlArray array) =>
        "[ " + string.Join(", ", array.Select(item => item?.ToString())) + " ]";

    private static bool TryGet(TomlTable table, string key, out object? value) =>
        table.TryGetValue(key, out value);

    public static string? GetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        return node?.ToString();
    }

    public static int GetTomlValueAsInt(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not long value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Int32)}");

        return Convert.ToInt32(value);
    }

    public static long GetTomlValueAsLong(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not long value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Int64)}");

        return value;
    }

    public static bool GetTomlValueAsBool(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not bool value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Boolean)}");

        return value;
    }

    public static double GetTomlValueAsFloat(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not double value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Double)}");

        return value;
    }

    public static DateTime GetTomlValueAsDateTime(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not TomlDateTime value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(DateTime)}");

        return value.DateTime.DateTime;
    }

    public static DateTimeOffset GetTomlValueAsDateTimeOffset(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (node is not TomlDateTime value)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(DateTime)}");

        return value.DateTime;
    }

    public static void SetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex, string value)
    {
        var container = string.IsNullOrWhiteSpace(sectionName)
            ? table
            : GetOrCreateSection(table, sectionName);

        if (!valueIndex.HasValue)
        {
            container[keyName] = value;
            return;
        }

        if (valueIndex.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(valueIndex), valueIndex, "Value must be zero or greater");

        if (!TryGet(container, keyName, out var existing) || existing is not TomlArray array)
        {
            array = new TomlArray();
            container[keyName] = array;
        }

        while (array.Count <= valueIndex.Value)
            array.Add(null);

        array[valueIndex.Value] = value;
    }

    private static TomlTable GetOrCreateSection(TomlTable table, string sectionName)
    {
        if (!TryGet(table, sectionName, out var existing) || existing is not TomlTable section)
        {
            section = new TomlTable();
            table[sectionName] = section;
        }

        return section;
    }
}
