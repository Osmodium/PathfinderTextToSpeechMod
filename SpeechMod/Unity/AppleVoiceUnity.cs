using Kingmaker;
using Kingmaker.Blueprints;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Unity;

public class AppleVoiceUnity : MonoBehaviour
{
    private static AppleVoiceUnity m_TheVoice;

    private static string GenderVoice => Game.Instance?.DialogController?.CurrentSpeaker?.Gender == Gender.Female ? Main.FemaleVoice : Main.MaleVoice;
    private static int GenderRate => Game.Instance?.DialogController?.CurrentSpeaker?.Gender == Gender.Female ? Main.Settings.FemaleRate : Main.Settings.MaleRate;

    private static bool IsVoiceInitialized()
    {
        if (m_TheVoice != null)
            return true;

        Main.Logger.Critical("No voice initialized!");
        return false;
    }

    void Start()
    {
        if (m_TheVoice != null)
            Destroy(gameObject);
        else
            m_TheVoice = this;
    }

    public static void Speak(string text, float delay = 0f)
    {
        if (!IsVoiceInitialized())
            return;

        if (delay > 0f)
        {
            m_TheVoice.ExecuteLater(delay, () => Speak(text));
            return;
        }

        Stop();

        Process.Start("/usr/bin/say", text);
    }

    public static void SpeakDialog(string text, float delay = 0f)
    {
        if (!IsVoiceInitialized())
            return;

        if (delay > 0f)
        {
            m_TheVoice.ExecuteLater(delay, () => SpeakDialog(text));
            return;
        }

        string arguments = "";
        text = new Regex("<b><color[^>]+><link([^>]+)?>([^<>]*)</link></color></b>").Replace(text, "$2");
        text = text.Replace("\\n", "  ");
        text = text.Replace("\n", " ");
        text = text.Replace(";", "");
        while (text.IndexOf("<color=#616060>", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            int position = text.IndexOf("<color=#616060>", StringComparison.InvariantCultureIgnoreCase);
            if (position != 0)
            {
                string argumentsPart = text.Substring(0, position);
                text = text.Substring(position);
                arguments = $"{arguments}say -v  {GenderVoice} -r {GenderRate} {argumentsPart.Replace("\"", "")};";
            }
            else
            {
                position = text.IndexOf("</color>", StringComparison.InvariantCultureIgnoreCase);
                string argumentsPart2 = text.Substring(0, position);
                text = text.Substring(position);
                arguments = $"{arguments}say -v {Main.NarratorVoice} -r {Main.Settings.NarratorRate} {argumentsPart2.Replace("\"", "")};";
            }
        }

        text = text.Replace("\"", "");
        if (!string.IsNullOrWhiteSpace(text) && text != "</color>")
            arguments = $"{arguments}say -v {GenderVoice} -r {GenderRate} {text};";

        arguments = new Regex("<[^>]+>").Replace(arguments, "");

        KillAll();

        arguments = "-c \"" + arguments + "\"";
        Process.Start("/bin/bash", arguments);
    }

    public static void Stop()
    {
        if (!IsVoiceInitialized())
            return;

        KillAll();
    }

    private static void KillAll()
    {
        Process.Start("/usr/bin/killall", "bash -kill");
        Process.Start("/usr/bin/killall", "say -kill");
    }
}