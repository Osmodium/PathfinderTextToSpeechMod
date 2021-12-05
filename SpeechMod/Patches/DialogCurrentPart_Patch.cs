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

            Debug.Log("Adding WindowsVoice gameobject.");

            var windowsVoiceGameObject = new GameObject();
            windowsVoiceGameObject.AddComponent<WindowsVoice>();

            Debug.Log("Adding speech button to current dialog ui.");

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
