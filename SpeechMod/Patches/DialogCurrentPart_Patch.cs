using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using SpeechMod.Unity;
using SpeechMod.Voice;
using UnityEngine;

namespace SpeechMod;

[HarmonyPatch(typeof(StaticCanvas), "Initialize")]
public static class DialogCurrentPart_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        AddDialogSpeechButton();
    }

    private static void AddDialogSpeechButton()
    {
        Debug.Log("Adding speech button to dialog ui.");

        var parent = Game.Instance.UI.Canvas.transform.TryFind("DialogPCView/Body/View/Scroll View");

        if (parent == null)
        {
            Debug.LogWarning("Parent not found!");
            return;
        }

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