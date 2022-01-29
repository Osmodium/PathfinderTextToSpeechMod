using System.Diagnostics;
using System.Text.RegularExpressions;
using Kingmaker;
using Kingmaker.Blueprints;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SpeechMod.Unity
{
    public class AppleVoiceUnity : MonoBehaviour
    {
        private static AppleVoiceUnity m_TheVoice;
        private Process m_SpeechProcess;

        void Start()
        {
            if (m_TheVoice != null)
                Destroy(gameObject);
            else
                m_TheVoice = this;
        }

        private static bool IsVoiceInitialized()
        {
            if (m_TheVoice != null)
                return true;

            Main.Logger.Critical("No voice initialized!");
            return false;
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

            m_TheVoice.m_SpeechProcess = Process.Start("/usr/bin/say", text);
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
            text = text.Replace(";", "");
            while (text.IndexOf("<color=#616060>") != -1)
            {
                int Position = text.IndexOf("<color=#616060>");
                if (Position != 0)
                {
                    string arguments_part = text.Substring(0, Position);
                    text = text.Substring(Position);
                    arguments = $"{arguments}say -v {Main.NarratorVoice} -r {Main.Settings.Rate} {arguments_part.Replace("\"", "")};";
                }
                else
                {
                    Position = text.IndexOf("</color>");
                    string arguments_part2 = text.Substring(0, Position);
                    text = text.Substring(Position);
                    arguments = $"{arguments}say -v {(Game.Instance?.DialogController?.CurrentSpeaker.Gender == Gender.Female ? Main.FemaleVoice : Main.MaleVoice)} -r {Main.Settings.Rate} {arguments_part2.Replace("\"", "")};";
                }
            }

            text = text.Replace("\"", "");
            if(!string.IsNullOrWhiteSpace(text))
                arguments = $"{arguments}say -v {Main.NarratorVoice} -r {Main.Settings.Rate} {text};";

            arguments = new Regex("<[^>]+>").Replace(arguments, "");

            Process.Start("/usr/bin/killall", "bash");
            Process.Start("/usr/bin/killall", "say");
            
            arguments = "-c \"" + arguments + "\"";
            m_TheVoice.m_SpeechProcess = Process.Start("/bin/bash", arguments);
        }

        public static void Stop()
        {
            if (!IsVoiceInitialized())
                return;

            if (m_TheVoice.m_SpeechProcess is { HasExited: false })
                m_TheVoice.m_SpeechProcess.Kill();
        }
    }
}
