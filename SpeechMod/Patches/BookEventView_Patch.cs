using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Dialog.BookEvent;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(BookEventView), "SetCues")]
public static class BookEventView_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
        Debug.Log($"{nameof(BookEventView)}_SetCues_Postfix @ {sceneName}");
#endif

        Transform cuesBlock = UIHelper.TryFindInCanvas("BookEventView/ContentWrapper/Window/Content/CuesBlock");
        if (cuesBlock == null)
        {
            Debug.LogWarning("CuesBlock not found!");
            return;
        }

        cuesBlock.HookupTextToSpeechOnTransform();
    }
}