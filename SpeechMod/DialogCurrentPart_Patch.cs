using System;
using System.IO;
using System.Reflection;
using System.Speech.Synthesis;
using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;

namespace SpeechMod
{
    [HarmonyPatch(typeof(StaticCanvas), "Initialize")]
    static class DialogCurrentPart_Patch
    {
        private static bool m_Initialized;
        private static Assembly m_SystemSpeech;
        //private static object m_SpeechSynthesizer;

        static void Postfix()
        {
            if (!Main.Enabled || m_Initialized)
                return;

            m_Initialized = true;

            Debug.Log(AssemblyDirectory);

            Debug.Log("Speech Mod Initializing...");

            var parent = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View");
            var originalButton = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View/ButtonEdge").gameObject;

            //if (parent == null || originalButton == null)
            //{
            //    m_Initialized = false;
            //    Debug.LogWarning("Parent or original button was not found!");
            //    return;
            //}

            var gameObject = GameObject.Instantiate(originalButton, parent);
            gameObject.name = "SpeechButton";
            gameObject.transform.localPosition = new Vector3(-493, 164, 0);
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

            var button = gameObject.GetComponent<OwlcatButton>();
            button.OnLeftClick.AddListener(Speak);

            gameObject.SetActive(true);

            Debug.Log("Adding 'System.Speech.dll'...");
            Console.Write("Adding 'System.Speech.dll'...");
            m_SystemSpeech = Assembly.LoadFrom($"{AssemblyDirectory}/System.Speech.dll");

            Debug.Log("Speech Mod Initialized!");
        }

        private static void Speak()
        {
            if (m_SystemSpeech == null)
            {
                Debug.LogWarning("Assembly not loaded!");
                return;
            }

            var speechSynthesizer = m_SystemSpeech.CreateInstance("System.Speech.Synthesis.SpeechSynthesizer");

            if (speechSynthesizer == null)
            {
                Debug.LogError("Could not instantiate speech synthesizer!");
                return;
            }

            ((SpeechSynthesizer)speechSynthesizer).Rate = Main.Settings?.Rate ?? -1;
            ((SpeechSynthesizer)speechSynthesizer).Volume = Main.Settings?.Volume ?? 100;
            //((SpeechSynthesizer)speechSynthesizer).SpeakCompleted += OnSpeakCompleted;

            ((SpeechSynthesizer)speechSynthesizer).SpeakAsync(Game.Instance.DialogController.CurrentCue.DisplayText);
        }

        //private static void OnSpeakCompleted(object sender, SpeakCompletedEventArgs e)
        //{
        //    ((SpeechSynthesizer)sender)?.Dispose();
        //}

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
