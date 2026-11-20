using FigLetText.Configuration;

namespace FigLetText.Services;

internal static class FigletRenderer
{
    private const int FallbackWidth = 80;

    public static void WriteAligned(string rendered, TextAlignmentType alignment)
    {
        var width = GetConsoleWidth();
        var lines = rendered.Split(["\r\n", "\n"], StringSplitOptions.None);

        foreach (var line in lines)
        {
            Console.Out.WriteLine(Align(line, alignment, width));
        }
    }

    private static string Align(string line, TextAlignmentType alignment, int width)
    {
        var padding = Math.Max(0, width - line.Length);

        return alignment switch
        {
            TextAlignmentType.Right => line.PadLeft(line.Length + padding),
            TextAlignmentType.Centre => line.PadLeft(line.Length + (padding / 2)),
            _ => line,
        };
    }

    private static int GetConsoleWidth()
    {
        try
        {
            return Console.IsOutputRedirected ? FallbackWidth : Console.WindowWidth;
        }
        catch
        {
            return FallbackWidth;
        }
    }
}
