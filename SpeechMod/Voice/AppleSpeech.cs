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
        var arguments = "-v '?' | awk '{$2=$3=\"\"; print $1}' | rev | cut -c 1- | rev";
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/say",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        Main.Logger.Log("Starting process...");
        process.Start();
        
        Main.Logger.Log("Process started...");
        
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        Main.Logger.Log("output: " + output);
        Main.Logger.Log("error: " + error);
        Main.Logger.Log("Read output waiting for exit...");
        
        process.WaitForExit();
        Main.Logger.Log("Disposing...");
        
        process.Dispose();
        Main.Logger.Log("Is null or whitespace check...");
        
        if (string.IsNullOrWhiteSpace(output))
            return null;
        Main.Logger.Log("Splitting...");
        

        return output.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
    }

    public string GetStatusMessage()
    {
        return "AppleSpeech ready!";
    }
}