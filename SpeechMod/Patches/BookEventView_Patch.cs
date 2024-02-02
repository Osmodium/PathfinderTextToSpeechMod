using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Dialog.BookEvent;
using Kingmaker.UI.MVVM._PCView.Dialog.Dialog;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

/// <summary>
/// Hooks TTS onto each paragraph in a book event.
/// </summary>
[HarmonyPatch(typeof(BookEventView<DialogAnswerPCView>), "SetCues")]
public static class BookEventView_Patch
{
    private const string CANVAS_CUES_BLOCK_PATH = "NestedCanvas1/BookEventPCView/ContentWrapper/Window/Content/CuesBlock";
    private const string GLOBALMAP_CUES_BLOCK_PATH = "BookEventView/ContentWrapper/Window/Content/CuesBlock";

    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;
        Debug.Log($"{nameof(BookEventView<DialogAnswerPCView>)}_SetCues_Postfix @ {sceneName}");
#endif

        Transform cuesBlock = UIHelper.TryFindInStaticCanvas(CANVAS_CUES_BLOCK_PATH, GLOBALMAP_CUES_BLOCK_PATH);

        if (cuesBlock == null)
        {
            Debug.LogWarning("CuesBlock not found!");
            return;
        }

        cuesBlock.HookTextToSpeechOnTransform();
    }
}