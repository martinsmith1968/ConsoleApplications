using SpecialFolders.Configuration;

namespace SpecialFolders.Writers.Interfaces;
public interface IWriter
{
    Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, Arguments arguments);
}
