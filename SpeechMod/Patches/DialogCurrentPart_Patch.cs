using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using SpeechMod.Unity;
using SpeechMod.Voice;
using UnityEngine;

namespace SpeechMod
{
    [HarmonyPatch(typeof(StaticCanvas), "Initialize")]
    public static class DialogCurrentPart_Patch
    {
        private static readonly string WindowsVoiceName = "WindowsVoice";

        public static void Postfix()
        {
            if (!Main.Enabled)
                return;

            AddUiElements();

            AddDialogSpeechButton();
        }
        
        private static void AddUiElements()
        {
            Debug.Log("Adding SpeechMod UI elements.");

            GameObject windowsVoice = null;
            try
            {
                windowsVoice = Object.FindObjectOfType<WindowsVoiceUnity>()?.gameObject;
            }
            catch{} // Sigh

            if (windowsVoice != null)
            {
                Debug.Log($"{nameof(WindowsVoiceUnity)} found!");
                return;
            }

            Debug.Log($"Adding {nameof(WindowsVoiceUnity)}...");

            var windowsVoiceGameObject = new GameObject(WindowsVoiceName);
            windowsVoiceGameObject.AddComponent<WindowsVoiceUnity>();
            Object.DontDestroyOnLoad(windowsVoiceGameObject);
        }

        private static void AddDialogSpeechButton()
        {
            Debug.Log("Adding speech button to dialog ui.");

            var parent = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View");

            var buttonGameObject = ButtonFactory.CreatePlayButton(parent, () =>
            {
                Speech.Speak(Game.Instance?.DialogController?.CurrentCue?.DisplayText);
            });

            buttonGameObject.name = "SpeechButton";
            buttonGameObject.transform.localPosition = new Vector3(-493, 164, 0);
            buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

            buttonGameObject.SetActive(true);
        }
    }
}
