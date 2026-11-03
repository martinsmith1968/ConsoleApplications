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

    public static TomlTable ReadText(string text)
    {
        return Toml.ToModel(text);
    }


    public static TomlObject GetTomlNode(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = !string.IsNullOrWhiteSpace(sectionName)
            ? table[sectionName] as TomlObject
            : table;

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

        return node;
    }

    public static string GetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        var node = GetTomlNode(table, sectionName, keyName, valueIndex);

        return (string)node;
    }

    public static int GetTomlValueAsInt(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static long GetTomlValueAsLong(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static bool GetTomlValueAsBool(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static double GetTomlValueAsFloat(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static DateTime GetTomlValueAsDateTime(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static DateTimeOffset GetTomlValueAsDateTimeOffset(TomlTable table, string sectionName, string keyName, int? valueIndex = null)
    {
        return default;
    }

    public static void SetTomlValue(TomlTable table, string sectionName, string keyName, int? valueIndex, string value)
    {

    }
}
