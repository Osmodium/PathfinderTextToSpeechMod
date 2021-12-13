using HarmonyLib;
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
#if DEBUG
            Debug.Log($"{nameof(DialogCueView)}_BindViewImplementation_Postfix");
#endif
            var cuesBlock = Game.Instance.UI.Canvas.transform.Find("BookEventPCView/ContentWrapper/Window/Content/CuesBlock");
            if (cuesBlock == null)
            {
                Debug.Log("CuesBlock not found!");
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
