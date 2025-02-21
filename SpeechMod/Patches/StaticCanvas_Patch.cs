using System;
using System.Security.Cryptography;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UI;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(StaticCanvas), "Initialize")]
public static class StaticCanvas_Patch
{
    private const string SCROLL_VIEW_PATH = "NestedCanvas1/DialogPCView/Body/View/Scroll View";

    public static void Postfix()
    {
        if (!Main.VoiceSettings.Enabled)
            return;

#if DEBUG
        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;
        Debug.Log($"{nameof(StaticCanvas)}_Initialize_Postfix @ {sceneName}");
#endif

        AddDialogSpeechButton();
    }

    private static void AddDialogSpeechButton()
    {

#if DEBUG
        Debug.Log("Adding speech button to dialog ui.");
#endif

        var parent = UIHelper.TryFindInStaticCanvas(SCROLL_VIEW_PATH);

        if (parent == null)
        {
            Debug.LogWarning("Parent not found!");
            return;
        }

        var buttonGameObject = ButtonFactory.CreatePlayButton(parent, () =>
        {
            var text = Game.Instance?.DialogController?.CurrentCue?.DisplayText ?? string.Empty;
            var speaker = Game.Instance?.DialogController?.CurrentSpeakerName;
            var gender = Game.Instance?.DialogController?.CurrentSpeaker?.Gender ?? Gender.Female;
            var key = Game.Instance?.DialogController?.CurrentCue?.Text?.Key ?? string.Empty;
            _ = VoicePlayer.PlayText(text, key, gender, speaker);
        });

        buttonGameObject.name = "SpeechButton";
        buttonGameObject.transform.localPosition = new Vector3(-493, 164, 0);
        buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

        buttonGameObject.SetActive(true);
    }
}