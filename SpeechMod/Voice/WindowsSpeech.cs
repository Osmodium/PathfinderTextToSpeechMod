using System;
using System.Linq;
using System.Text.RegularExpressions;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using SpeechMod.Unity;

#if DEBUG
using System.Reflection;
#endif

namespace SpeechMod.Voice;

public class WindowsSpeech : ISpeech
{
    private static string NarratorVoice => $"<voice required=\"Name={Main.NarratorVoice}\">";
    private static string NarratorPitch => $"<pitch absmiddle=\"{Main.Settings.NarratorPitch}\"/>";
    private static string NarratorRate => $"<rate absspeed=\"{Main.Settings.NarratorRate}\"/>";
    private static string NarratorVolume => $"<volume level=\"{Main.Settings.NarratorVolume}\"/>";

    private static string FemaleVoice => $"<voice required=\"Name={Main.FemaleVoice}\">";
    private static string FemaleVolume => $"<volume level=\"{Main.Settings.FemaleVolume}\"/>";
    private static string FemalePitch => $"<pitch absmiddle=\"{Main.Settings.FemalePitch}\"/>";
    private static string FemaleRate => $"<rate absspeed=\"{Main.Settings.FemaleRate}\"/>";

    private static string MaleVoice => $"<voice required=\"Name={Main.MaleVoice}\">";
    private static string MaleVolume => $"<volume level=\"{Main.Settings.MaleVolume}\"/>";
    private static string MalePitch => $"<pitch absmiddle=\"{Main.Settings.MalePitch}\"/>";
    private static string MaleRate => $"<rate absspeed=\"{Main.Settings.MaleRate}\"/>";

    private static string ProtagonistVoice => $"<voice required=\"Name={Main.ProtagonistVoice}\">";
    private static string ProtagonistVolume => $"<volume level=\"{Main.Settings.ProtagonistVolume}\"/>";
    private static string ProtagonistPitch => $"<pitch absmiddle=\"{Main.Settings.ProtagonistPitch}\"/>";
    private static string ProtagonistRate => $"<rate absspeed=\"{Main.Settings.ProtagonistRate}\"/>";

    public string CombinedNarratorVoiceStart => $"{NarratorVoice}{NarratorPitch}{NarratorRate}{NarratorVolume}";
    public string CombinedFemaleVoiceStart => $"{FemaleVoice}{FemalePitch}{FemaleRate}{FemaleVolume}";
    public string CombinedMaleVoiceStart => $"{MaleVoice}{MalePitch}{MaleRate}{MaleVolume}";
    public string CombinedProtagonistVoiceStart => $"{ProtagonistVoice}{ProtagonistPitch}{ProtagonistRate}{ProtagonistVolume}";

    public virtual string CombinedDialogVoiceStart
    {
        get
        {
            if (Game.Instance?.DialogController?.CurrentSpeaker == null)
                return CombinedNarratorVoiceStart;

            if (Game.Instance?.DialogController?.CurrentSpeaker.IsMainCharacter == true)
                return CombinedProtagonistVoiceStart;

            return Game.Instance?.DialogController?.CurrentSpeaker.Gender switch
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

    private void SpeakInternal(string text, float delay = 0f)
    {
        text = "<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xmlns:mstts=\"http://www.w3.org/2001/mstts\">" + text + "</speak>";
        //if (Main.Settings?.LogVoicedLines == true)
        //    UnityEngine.Debug.Log(text);
        WindowsVoiceUnity.Speak(text, Length(text), delay);
    }

    public bool IsSpeaking()
    {
        return WindowsVoiceUnity.IsSpeaking;
    }

    public void SpeakPreview(string text, VoiceType voiceType)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = text.PrepareText();
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
            case VoiceType.Protagonist:
                text = $"{CombinedProtagonistVoiceStart}{text}</voice>";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(voiceType), voiceType, null);
        }

        WindowsVoiceUnity.Speak(text, Length(text));
    }

    public string PrepareSpeechText(string text)
    {
#if DEBUG
        UnityEngine.Debug.Log(text);
#endif
        text = new Regex("<[^>]+>").Replace(text, "");
        text = text.PrepareText();
        text = $"{CombinedNarratorVoiceStart}{text}</voice>";

#if DEBUG
        if (Assembly.GetEntryAssembly() == null)
            UnityEngine.Debug.Log(text);
#endif
        return text;
    }

    public string PrepareDialogText(string text)
    {
        text = text.PrepareText();

        text = new Regex("<b><color[^>]+><link([^>]+)?>([^<>]*)</link></color></b>").Replace(text, "$2");

#if DEBUG
        if (Assembly.GetEntryAssembly() == null)
            UnityEngine.Debug.Log(text);
#endif

        text = FormatGenderSpecificVoices(text);

#if DEBUG
        if (Assembly.GetEntryAssembly() == null)
            UnityEngine.Debug.Log(text);
#endif

        return text;
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

        text = PrepareDialogText(text);

        WindowsVoiceUnity.Speak(text, Length(text), delay);
    }

    public void SpeakAs(string text, VoiceType voiceType, float delay = 0)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        if (Main.Settings!.UseProtagonistSpecificVoice && voiceType == VoiceType.Protagonist)
        {
            text = $"{CombinedProtagonistVoiceStart}{text}</voice>";
            SpeakInternal(text, delay);
            return;
        }

        if (!Main.Settings!.UseGenderSpecificVoices)
        {
            Speak(text, delay);
            return;
        }

        text = voiceType switch
        {
            VoiceType.Narrator => $"{CombinedNarratorVoiceStart}{text}</voice>",
            VoiceType.Female => $"{CombinedFemaleVoiceStart}{text}</voice>",
            VoiceType.Male => $"{CombinedMaleVoiceStart}{text}</voice>",
            _ => throw new ArgumentOutOfRangeException(nameof(voiceType), voiceType, null)
        };

        SpeakInternal(text, delay);
    }

    public void Speak(string text, float delay = 0f)
    {
        if (string.IsNullOrEmpty(text))
        {
            Main.Logger?.Warning("No text to speak!");
            return;
        }

        text = PrepareSpeechText(text);

        WindowsVoiceUnity.Speak(text, Length(text), delay);
    }

    public void Stop()
    {
        WindowsVoiceUnity.Stop();
    }

    public void NextPhrase()
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