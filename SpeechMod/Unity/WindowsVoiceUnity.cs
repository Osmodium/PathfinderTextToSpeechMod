﻿using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SpeechMod.Unity
{
    /// <summary>
    /// Credit to Chad Weisshaar for the base from https://chadweisshaar.com/blog/2015/07/02/microsoft-speech-for-unity/
    /// </summary>

    public static class Utility
    {
        public static Coroutine ExecuteLater(this MonoBehaviour behaviour, float delay, Action action)
        {
            return behaviour.StartCoroutine(_realExecute(delay, action));
        }
        static IEnumerator _realExecute(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }

    public class WindowsVoiceUnity : MonoBehaviour
    {
        [DllImport("WindowsVoice")]
        private static extern void initSpeech(int rate, int volume);
        [DllImport("WindowsVoice")]
        private static extern void destroySpeech();
        [DllImport("WindowsVoice")]
        private static extern void addToSpeechQueue(string s);
        [DllImport("WindowsVoice")]
        private static extern void clearSpeechQueue();
        [DllImport("WindowsVoice")]
        private static extern string getStatusMessage();
        [DllImport("WindowsVoice")]
        private static extern string getVoicesAvailable();

        private static WindowsVoiceUnity m_TheVoice;

        public static bool IsSpeaking
        {
            get
            {
                string theStatus = getStatusMessage();
                return !string.IsNullOrWhiteSpace(theStatus) && theStatus.Equals("Speaking");
            }
        }

        private static void Init()
        {
            initSpeech(1, 100);
        }

        void Start()
        {
            if (m_TheVoice == null)
            {
                m_TheVoice = this;
                Init();
            }
            else
                Destroy(gameObject);
        }

        public static string[] GetAvailableVoices()
        {
            string voicesDelim = getVoicesAvailable();
            if (string.IsNullOrWhiteSpace(voicesDelim))
                return Array.Empty<string>();
            string[] voices = voicesDelim.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < voices.Length; ++i)
            {
                if (!voices[i].Contains('-'))
                    continue;
                voices[i] = voices[i].Substring(0, voices[i].IndexOf('-')).Trim();
            }
            return voices;
        }

        public static void Speak(string msg, float delay = 0f)
        {
            if (delay == 0f)
                addToSpeechQueue(msg);
            else
                m_TheVoice.ExecuteLater(delay, () => Speak(msg));
        }

        public static string GetStatusMessage()
        {
            return getStatusMessage();
        }

        public static void Stop()
        {
            if (!IsSpeaking)
                return;

            destroySpeech();
            Init();
        }

        public static void ClearQueue()
        {
            clearSpeechQueue();
        }

        void OnDestroy()
        {
            if (m_TheVoice == this)
            {
                Debug.Log("Destroying speech");
                destroySpeech();
                Debug.Log("Speech destroyed");
                m_TheVoice = null;
            }
        }
    }
}