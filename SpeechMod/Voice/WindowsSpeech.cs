using System;
using Kingmaker;
using Kingmaker.Blueprints;
using SpeechMod.Unity;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpeechMod.Voice;

public class WindowsSpeech : ISpeech
{
    private static string NarratorVoice => $"<voice required=\"Name={ Main.NarratorVoice }\">";
    private static string FemaleVoice => $"<voice required=\"Name={ Main.FemaleVoice }\">";
    private static string MaleVoice => $"<voice required=\"Name={ Main.MaleVoice }\">";
    private static string NarratorPitch => $"<pitch absmiddle=\"{ Main.Settings.NarratorPitch }\"/>";
    private static string NarratorRate => $"<rate absspeed=\"{ Main.Settings.NarratorRate }\"/>";
    private static string NarratorVolume => $"<volume level=\"{ Main.Settings.NarratorVolume }\"/>";
    private static string FemaleVolume => $"<volume level=\"{ Main.Settings.FemaleVolume }\"/>";
    private static string FemalePitch => $"<pitch absmiddle=\"{ Main.Settings.FemalePitch }\"/>";
    private static string FemaleRate => $"<rate absspeed=\"{ Main.Settings.FemaleRate }\"/>";
    private static string MaleVolume => $"<volume level=\"{ Main.Settings.MaleVolume }\"/>";
    private static string MalePitch => $"<pitch absmiddle=\"{ Main.Settings.MalePitch }\"/>";
    private static string MaleRate => $"<rate absspeed=\"{ Main.Settings.MaleRate }\"/>";
    
    private string CombinedNarratorVoiceStart => $"{NarratorVoice}{NarratorPitch}{NarratorRate}{NarratorVolume}";
    private string CombinedFemaleVoiceStart => $"{FemaleVoice}{FemalePitch}{FemaleRate}{FemaleVolume}";
    private string CombinedMaleVoiceStart => $"{MaleVoice}{MalePitch}{MaleRate}{MaleVolume}";

    private string CombinedDialogVoiceStart
    {
        get
        {
            if (Game.Instance?.DialogController?.CurrentSpeaker == null)
                return CombinedNarratorVoiceStart;

            return Game.Instance.DialogController.CurrentSpeaker.Gender switch
            {
                Gender.Female => CombinedFemaleVoiceStart,
                Gender.Male => CombinedMaleVoiceStart,
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

    public void SpeakPreview(string text, VoiceType voiceType)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = text.PrepareSpeechText();
        text = new Regex("<[^>]+>").Replace(text, "");

        switch (voiceType)
        {
            case VoiceType.Narrator:
                text = $"{CombinedNarratorVoiceStart}{text}</voice>";
                break;
            case VoiceType.Female:
                text = $"{CombinedFemaleVoiceStart}{text}</voice>";
                break;
            case VoiceType.Male:
                text = $"{CombinedMaleVoiceStart}{text}</voice>";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(voiceType), voiceType, null);
        }

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
        text = new Regex("<[^>]+>").Replace(text, "");
        text = text.PrepareSpeechText();
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