using SpeechMod.Unity;
using System.Linq;
using System.Text.RegularExpressions;
using Kingmaker;
using Kingmaker.Blueprints;

namespace SpeechMod.Voice;

public class WindowsSpeech : ISpeech
{
    private static string NarratorVoice => $"<voice required=\"Name={ Main.NarratorVoice }\">";
    private static string FemaleVoice => $"<voice required=\"Name={ Main.FemaleVoice }\">";
    private static string MaleVoice => $"<voice required=\"Name={ Main.MaleVoice }\">";
    private static string Pitch => $"<pitch absmiddle=\"{ Main.Settings.Pitch }\"/>";
    private static string Rate => $"<rate absspeed=\"{ Main.Settings.Rate }\"/>";
    private static string Volume => $"<volume level=\"{ Main.Settings.Volume }\"/>";

    private string CombinedNarratorVoiceStart => $"{NarratorVoice}{Pitch}{Rate}{Volume}";

    private string CombinedDialogVoiceStart
    {
        get
        {
            if (Game.Instance?.DialogController?.CurrentSpeaker == null)
                return CombinedNarratorVoiceStart;

            return Game.Instance.DialogController.CurrentSpeaker.Gender switch
            {
                Gender.Male => $"{MaleVoice}{Pitch}{Rate}{Volume}",
                Gender.Female => $"{FemaleVoice}{Pitch}{Rate}{Volume}",
                _ => CombinedNarratorVoiceStart
            };
        }
    }

    public static int Length(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var arr = new[] { "—", "-", "\"" };

        return arr.Aggregate(text, (current, t) => current.Replace(t, "")).Length;
    }

    private string FormatGenderSpecificVoices(string text)
    {
        text = text.Replace("<color=#616060>", $"</voice>{CombinedNarratorVoiceStart}");
        text = text.Replace("</color>", $"</voice>{CombinedDialogVoiceStart}");
        
        if (text.StartsWith("</voice>"))
            text = text.Remove(0, 8);
        else
            text = CombinedDialogVoiceStart + text;
        
        if (text.EndsWith(CombinedDialogVoiceStart))
            text = text.Remove(text.Length - CombinedDialogVoiceStart.Length, CombinedDialogVoiceStart.Length);
	
        if (!text.EndsWith("</voice>"))
            text += "</voice>";
        return text;
    }

    public void SpeakPreview(string text, string voice)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = text.PrepareSpeechText();
        text = new Regex("<[^>]+>").Replace(text, "");
        text = $"<voice required=\"Name={voice}\">{Pitch}{Rate}{Volume}{text}</voice>";
        WindowsVoiceUnity.Speak(text, Length(text));
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

        text = new Regex("<b><color[^>]+><link([^>]+)?>([^<>]*)</link></color></b>").Replace(text, "$2");

#if DEBUG
        UnityEngine.Debug.Log(text);
#endif

        text = FormatGenderSpecificVoices(text);

#if DEBUG
        UnityEngine.Debug.Log(text);
#endif

        WindowsVoiceUnity.Speak(text, Length(text), delay);
    }

    public void Speak(string text, float delay = 0f)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

#if DEBUG
        UnityEngine.Debug.Log(text);
#endif
        text = text.PrepareSpeechText();
        text = new Regex("<[^>]+>").Replace(text, "");
        text = $"{CombinedNarratorVoiceStart}{text}</voice>";

#if DEBUG
        UnityEngine.Debug.Log(text);
#endif
        WindowsVoiceUnity.Speak(text, Length(text), delay);
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