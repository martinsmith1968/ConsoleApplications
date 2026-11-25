using SpecialFolders.Commands;

namespace SpecialFolders.Writers.Interfaces;
public interface IWriter
{
    Task WriteOutputAsync(List<KeyValuePair<Environment.SpecialFolder, string>> items, SpecialFoldersCommand.Settings settings);
}
