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
        if (!Main.Enabled)
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
            Main.WaveOutEvent?.Stop();
            using var md5 = MD5.Create();
            var inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
            var guid = new Guid(md5.ComputeHash(inputBytes));
            _ = VoicePlayer.PlayText(text, guid.ToString(), Gender.Female, "narrator");
        });

        buttonGameObject.name = "SpeechButton";
        buttonGameObject.transform.localPosition = new Vector3(-493, 164, 0);
        buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

        buttonGameObject.SetActive(true);
    }
}