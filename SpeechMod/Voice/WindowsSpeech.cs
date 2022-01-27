using SpeechMod.Unity;
using System.Linq;

namespace SpeechMod.Voice;

public class WindowsSpeech : ISpeech
{
    private static string Voice => $"<voice required=\"Name={ Main.ChosenVoice }\">";
    private static string Pitch => $"<pitch absmiddle=\"{ Main.Settings.Pitch }\"/>";
    private static string Rate => $"<rate absspeed=\"{ Main.Settings.Rate }\"/>";
    private static string Volume => $"<volume level=\"{ Main.Settings.Volume }\"/>";

    public static int Length(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var arr = new[] { "—", "-", "\"" };

        return arr.Aggregate(text, (current, t) => current.Replace(t, "")).Length;
    }

    public void Speak(string text, float delay = 0f)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No display text in the curren cue of the dialog controller!");
            return;
        }
            
#if DEBUG
        UnityEngine.Debug.Log(text);
#endif
        text = text.PrepareSpeechText();

        string textToSpeak = $"{ Voice }{ Pitch }{ Rate }{ Volume }{ text }</voice>";

#if DEBUG
        UnityEngine.Debug.Log(textToSpeak);
#endif
        WindowsVoiceUnity.Speak(textToSpeak, Length(textToSpeak), delay);
    }

    public void Stop()
    {
        WindowsVoiceUnity.Stop();
    }

    public string[] GetAvailableVoices()
    {
        return WindowsVoiceUnity.GetAvailableVoices();
    }

    public string GetStatusMessage()
    {
        return WindowsVoiceUnity.GetStatusMessage();
    }
}