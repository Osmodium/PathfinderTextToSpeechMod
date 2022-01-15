﻿using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Dialog.BookEvent;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches
{
    [HarmonyPatch(typeof(BookEventView), "SetCues")]
    public static class BookEventView_Patch
    {
        public static void Postfix()
        {
            var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
            Debug.Log($"{nameof(BookEventView)}_SetCues_Postfix @ {sceneName}");
#endif
            Transform cuesBlock = null;

            if (sceneName == "UI_Globalmap_Scene")
                cuesBlock = Game.Instance.UI.GlobalMapUI.transform.TryFind("BookEventView/ContentWrapper/Window/Content/CuesBlock"); // In map   
            else if (sceneName == "UI_Ingame_Scene")
                cuesBlock = Game.Instance.UI.Canvas.transform.TryFind("BookEventPCView/ContentWrapper/Window/Content/CuesBlock"); // Normal

            if (cuesBlock == null)
            {
                Debug.LogWarning("CuesBlock not found!");
                return;
            }

            var allTexts = cuesBlock.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in allTexts)
            {
                text.HookupTextToSpeech();
            }
        }
    }
}
