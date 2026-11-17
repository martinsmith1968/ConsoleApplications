using System.IO;

namespace Toaster.Tests;

public class SoundParserTests
{
    // Minimal valid (silent, zero-length) PCM WAV: a 44-byte RIFF/WAVE header, no sample data.
    private static readonly byte[] SilentWavBytes =
    {
        (byte)'R', (byte)'I', (byte)'F', (byte)'F', 36, 0, 0, 0, (byte)'W', (byte)'A', (byte)'V', (byte)'E',
        (byte)'f', (byte)'m', (byte)'t', (byte)' ', 16, 0, 0, 0, 1, 0, 1, 0,
        0x40, 0x1F, 0, 0, 0x40, 0x1F, 0, 0, 1, 0, 8, 0,
        (byte)'d', (byte)'a', (byte)'t', (byte)'a', 0, 0, 0, 0,
    };

    private static string CreateTempWavFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");
        File.WriteAllBytes(path, SilentWavBytes);
        return path;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_true_for_no_sound_requested(string? text)
    {
        SoundParser.IsValid(text).ShouldBeTrue();
    }

    [Theory]
    [InlineData("Asterisk")]
    [InlineData("BEEP")]
    [InlineData("Exclamation")]
    [InlineData("hand")]
    [InlineData("Question")]
    public void IsValid_true_for_known_system_sound_name(string text)
    {
        SoundParser.IsValid(text).ShouldBeTrue();
    }

    [Fact]
    public void IsValid_true_for_an_existing_wav_file()
    {
        var path = CreateTempWavFile();
        try
        {
            SoundParser.IsValid(path).ShouldBeTrue();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsValid_false_for_a_non_existent_wav_file()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.wav");

        SoundParser.IsValid(path).ShouldBeFalse();
    }

    [Fact]
    public void IsValid_false_for_an_existing_file_with_the_wrong_extension()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.mp3");
        File.WriteAllText(path, "not a wav");
        try
        {
            SoundParser.IsValid(path).ShouldBeFalse();
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void IsValid_false_for_an_unknown_name()
    {
        SoundParser.IsValid("NotASound").ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryPlay_does_nothing_when_no_sound_requested(string? text)
    {
        Should.NotThrow(() => SoundParser.TryPlay(text));
    }

    [Fact]
    public void TryPlay_does_not_throw_for_a_known_system_sound_name()
    {
        Should.NotThrow(() => SoundParser.TryPlay("Asterisk"));
    }

    [Fact]
    public void TryPlay_does_not_throw_for_an_existing_wav_file()
    {
        var path = CreateTempWavFile();
        try
        {
            Should.NotThrow(() => SoundParser.TryPlay(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void TryPlay_does_not_throw_for_garbage()
    {
        Should.NotThrow(() => SoundParser.TryPlay("NotASound"));
    }
}
