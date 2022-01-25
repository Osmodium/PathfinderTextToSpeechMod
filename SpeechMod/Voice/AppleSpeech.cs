using SpeechMod.Unity;

namespace SpeechMod.Voice;

public class AppleSpeech : ISpeech
{
    public void Speak(string text)
    {
        AppleVoiceUnity.Speak(text);
    }

    public string[] GetAvailableVoices()
    {
        return new[]
        {
            "Anna",
            "Markus",
            "Petra"
        };
    }

    public string GetStatusMessage()
    {
        return "AppleSpeech ready!";
    }
}