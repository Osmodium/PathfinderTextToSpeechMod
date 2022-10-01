using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Dialog.BookEvent;
using Kingmaker.UI.MVVM._PCView.Dialog.Dialog;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(BookEventView<DialogAnswerPCView>), "SetCues")]
public static class BookEventView_Patch
{
    private const string CUES_BLOCK_PATH = "NestedCanvas1/BookEventPCView/ContentWrapper/Window/Content/CuesBlock";

    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
        Debug.Log($"{nameof(BookEventView<DialogAnswerPCView>)}_SetCues_Postfix @ {sceneName}");
#endif

        Transform cuesBlock = UIHelper.TryFindInStaticCanvas(CUES_BLOCK_PATH);

        if (cuesBlock == null)
        {
            Debug.LogWarning("CuesBlock not found!");
            return;
        }

        cuesBlock.HookupTextToSpeechOnTransform();
    }
}