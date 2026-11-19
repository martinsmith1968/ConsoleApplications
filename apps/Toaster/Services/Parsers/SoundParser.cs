using System.IO;
using System.Media;

namespace Toaster.Services.Parsers;

/// <summary>Resolves the optional sound to play when a toast appears: a named system sound, or a .wav file path.</summary>
internal static class SoundParser
{
    private static readonly Dictionary<string, SystemSound> KnownSystemSounds = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(SystemSounds.Asterisk)]    = SystemSounds.Asterisk,
        [nameof(SystemSounds.Beep)]        = SystemSounds.Beep,
        [nameof(SystemSounds.Exclamation)] = SystemSounds.Exclamation,
        [nameof(SystemSounds.Hand)]        = SystemSounds.Hand,
        [nameof(SystemSounds.Question)]    = SystemSounds.Question,
    };

    /// <summary>True if empty (no sound requested), a known system sound name, or an existing .wav file path.</summary>
    public static bool IsValid(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return true;

        text = text.Trim();

        return KnownSystemSounds.ContainsKey(text) || IsExistingWavFile(text);
    }

    /// <summary>Plays the sound (fire-and-forget); does nothing if empty, and swallows any playback failure.</summary>
    public static void TryPlay(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        text = text.Trim();

        try
        {
            if (KnownSystemSounds.TryGetValue(text, out var systemSound))
                systemSound.Play();
            else if (IsExistingWavFile(text))
                new SoundPlayer(text).Play();
        }
        catch
        {
            // Best-effort: a missing/corrupt/locked sound file shouldn't stop the toast appearing.
        }
    }

    private static bool IsExistingWavFile(string text) =>
        text.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) && File.Exists(text);
}
