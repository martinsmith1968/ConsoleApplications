using Tommy;

namespace TomlFileHandler.Services;

public class TommyTOMLFileService
{
    public static TomlTable ReadFile(string fileName)
    {
        using var reader = File.OpenText(fileName);

        return TOML.Parse(reader);
    }

    public static TomlTable ReadText(string text)
    {
        using var reader = new StringReader(text);

        return TOML.Parse(reader);
    }

    public static void WriteFile(TomlTable table, string fullName)
    {
        using var writer = File.CreateText(fullName);

        table.WriteTo(writer);

        writer.Flush();
    }

    public static TomlNode GetTomlNode(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = !string.IsNullOrWhiteSpace(sectionName)
            ? table[sectionName][keyName]
            : table[keyName];

        if (valueIndex.HasValue)
        {
            if (node is not TomlArray valueArray)
            {
                throw new ArgumentOutOfRangeException(keyName, valueIndex, $"Cannot access Index: {valueIndex} - '{keyName}' is not an array");
            }

            var arrayIndex = valueIndex.Value >= 0
                ? valueIndex.Value
                : valueArray.ChildrenCount + valueIndex.Value;

            node = valueArray[arrayIndex];
        }

        return node;
    }

    public static string GetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return GetTomlNode(table, sectionName, keyName, valueIndex)
            ?.ToString();
    }

    public static int GetTomlValueAsInt(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsInteger) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Int32)}");

        return Convert.ToInt32(node!.AsInteger.Value);
    }

    public static long GetTomlValueAsLong(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsInteger) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Int64)}");

        return node!.AsInteger.Value;
    }

    public static bool GetTomlValueAsBool(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsBoolean) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Boolean)}");

        return node!.AsBoolean.Value;
    }

    public static double GetTomlValueAsFloat(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsFloat) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(Double)}");

        return node!.AsFloat.Value;
    }

    public static DateTime GetTomlValueAsDateTime(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsDateTime) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(DateTime)}");

        return node!.AsDateTimeLocal.Value;
    }

    public static DateTimeOffset GetTomlValueAsDateTimeOffset(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        if (!(node?.IsDateTime) ?? false)
            throw new InvalidCastException($"Key: '{keyName}' is not an {nameof(DateTime)}");

        return node!.AsDateTimeOffset.Value;
    }

    public static void SetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex, string value)
    {
        // TODO: Handle Arrays / ValueIndex

        var node = string.IsNullOrWhiteSpace(sectionName)
            ? table
            : table[sectionName];

        if (valueIndex.HasValue)
        {
            node ??= new TomlArray();
            if (!node.IsArray)
                throw new ArgumentOutOfRangeException(nameof(keyName), "Specified Key is not an array");

            node = node.AsArray;

            while (node.ChildrenCount <= valueIndex.Value)
                node.Add(null);

            node[valueIndex.Value] = value;
        }
        else
        {
            node[keyName] = value;
        }
    }
}
