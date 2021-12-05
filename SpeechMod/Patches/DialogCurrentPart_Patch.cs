using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using UnityEngine;

namespace SpeechMod
{
    [HarmonyPatch(typeof(StaticCanvas), "Initialize")]
    static class DialogCurrentPart_Patch
    {
        static void Postfix()
        {
            if (!Main.Enabled)
                return;

            AddWindowsVoice();

            AddDialogSpeechButton();
        }

        private static void AddWindowsVoice()
        {
            Debug.Log("Adding WindowsVoice gameobject.");

            var windowsVoiceGameObject = new GameObject("WindowsVoice");
            windowsVoiceGameObject.AddComponent<WindowsVoice>();
            GameObject.DontDestroyOnLoad(windowsVoiceGameObject);
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
