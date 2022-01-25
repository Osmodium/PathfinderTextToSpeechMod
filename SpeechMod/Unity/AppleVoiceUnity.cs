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
            text = new Regex("<[^>]+>").Replace(text, "");
            //Process.Start("/usr/bin/killall", "say");
            m_TheVoice.speechProcess.Kill();
            string arguments = string.Concat("-v ", Main.ChosenVoice, " -r ", Main.Settings.Rate.ToString(), " ", text.Replace("\"", ""));
            m_TheVoice.speechProcess = Process.Start("/usr/bin/say", arguments);
        }
    }
}
