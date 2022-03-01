using SpeechMod.Unity;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpeechMod.Voice;

public class AppleSpeech : ISpeech
{
    public void SpeakPreview(string text, VoiceType type)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        switch (type)
        {
            case VoiceType.Narrator:
                text = $"-v {Main.Settings.NarratorVoice} -r {Main.Settings.NarratorRate} {text.Replace("\"", "")}";
                break;
            case VoiceType.Female:
                text = $"-v {Main.Settings.FemaleVoice} -r {Main.Settings.FemaleRate} {text.Replace("\"", "")}";
                break;
            case VoiceType.Male:
                text = $"-v {Main.Settings.MaleVoice} -r {Main.Settings.MaleRate} {text.Replace("\"", "")}";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

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

        text = text.PrepareText();
        AppleVoiceUnity.SpeakDialog(text, delay);
    }

    public void Speak(string text, float delay)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = text.PrepareText();
        text = new Regex("<[^>]+>").Replace(text, "");
        text = $"-v {Main.NarratorVoice} -r {Main.Settings.NarratorRate} {text.Replace("\"", "")}";
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