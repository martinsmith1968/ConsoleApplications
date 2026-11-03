using SpecialFolders.Configuration.Types;
using SpecialFolders.Writers.Interfaces;

namespace SpecialFolders.Writers;
internal class WriterFactory
{
    public static IWriter CreateWriter(OutputWriterType writerType)
    {
        return writerType switch
        {
            OutputWriterType.Grid => new GridWriter(),
            OutputWriterType.Table => new TableWriter(),
            _ => new StandardTextWriter()
        };
    }
}
