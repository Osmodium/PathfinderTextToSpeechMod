using SpeechMod.Unity;
using System;
using System.Diagnostics;

namespace SpeechMod.Voice;

public class AppleSpeech : ISpeech
{
    public void Speak(string text)
    {
        AppleVoiceUnity.Speak(text);
    }

    public string[] GetAvailableVoices()
    {
        string arguments = "say -v '?' | awk '{\\$3=\\\"\\\"; printf \\\"%s;\\\", \\$1\\\"#\\\"\\$2}' | rev | cut -c 2- | rev";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"" + arguments + "\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        string error = process.StandardError.ReadToEnd();
        string text = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        process.Dispose();

        if (string.IsNullOrWhiteSpace(text))
        {
#if DEBUG
            Main.Logger.Warning($"[GetAvailableVoices] {error}");
#endif
            return null;
        }

        return text.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
    }

    public string GetStatusMessage()
    {
        return "AppleSpeech ready!";
    }
}