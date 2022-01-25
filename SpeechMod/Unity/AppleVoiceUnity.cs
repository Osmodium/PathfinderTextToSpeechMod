using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SpeechMod.Unity
{
    public class AppleVoiceUnity : MonoBehaviour
    {
        private static AppleVoiceUnity m_TheVoice;
        private Process speechProcess;

        void Start()
        {
            if (m_TheVoice != null)
                Destroy(gameObject);
            else
                m_TheVoice = this;
        }
        
        public static void Speak(string text)
        {
            if (m_TheVoice == null)
            {
                Main.Logger.Critical("No voice initialized!");
                return;
            }

            text = new Regex("<[^>]+>").Replace(text, "");

            if (m_TheVoice.speechProcess is { HasExited: false })
                m_TheVoice.speechProcess.Kill();

            string arguments = string.Concat("-v ", Main.ChosenVoice, " -r ", Main.Settings.Rate.ToString(), " ", text.Replace("\"", ""));
            m_TheVoice.speechProcess = Process.Start("/usr/bin/say", arguments);
        }
    }
}
