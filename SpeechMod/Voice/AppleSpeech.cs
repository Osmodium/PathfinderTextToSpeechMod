using SpeechMod.Unity;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpeechMod.Voice;

public class AppleSpeech : ISpeech
{
    public void SpeakPreview(string text, string voice)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = $"-v {voice} -r {Main.Settings.Rate} {text.Replace("\"", "")}";
        AppleVoiceUnity.Speak(text);
    }

    public void SpeakDialog(string text, float delay = 0f)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        if (!Main.Settings.UseGenderSpecificVoices)
        {
            Speak(text, delay);
            return;
        }

        text = text.PrepareSpeechText();
        AppleVoiceUnity.SpeakDialog(text, delay);
    }

    public void Speak(string text, float delay)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = text.PrepareSpeechText();
        text = new Regex("<[^>]+>").Replace(text, "");
        text = $"-v {Main.NarratorVoice} -r {Main.Settings.Rate} {text.Replace("\"", "")}";
        AppleVoiceUnity.Speak(text, delay);
    }
    
    public void Stop()
    {
        AppleVoiceUnity.Stop();
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