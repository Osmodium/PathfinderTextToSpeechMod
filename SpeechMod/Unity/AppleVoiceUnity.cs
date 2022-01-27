﻿using System.Diagnostics;
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

            if (m_TheVoice.speechProcess is { HasExited: false })
                m_TheVoice.speechProcess.Kill();

            string arguments = string.Concat("-v ", Main.ChosenVoice, " -r ", Main.Settings.Rate.ToString(), " ", text.Replace("\"", ""));
            m_TheVoice.speechProcess = Process.Start("/usr/bin/say", arguments);
        }

        public static void Stop()
        {
            if (!IsVoiceInitialized())
                return;

            if (m_TheVoice.speechProcess is { HasExited: false })
                m_TheVoice.speechProcess.Kill();
        }
    }
}
