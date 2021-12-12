using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using System;
using SpeechMod.Unity;
using SpeechMod.Voice;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SpeechMod
{
    [HarmonyPatch(typeof(TooltipEngine), "GetBrickView")]
    static class TooltipEngine_Patch
    {
        static void Postfix(ref MonoBehaviour __result)
        {
            if (!Main.Enabled)
                return;

            if (__result == null)
                return;

            // TODO: Possibly add more types
            if (!(__result is TooltipBrickTextView view))
                return;

            if (IsInvalid(view.transform?.parent))
                return;

            var textMeshProTransform = view.gameObject?.transform?.Find("Text (TMP)");
            if (textMeshProTransform == null)
            {
                Debug.LogWarning("No TextMeshProUGUI found!");
                return;
            }

            var textMeshPro = textMeshProTransform.GetComponent<TextMeshProUGUI>();
            if (textMeshPro == null)
            {
                Debug.LogWarning("No TextMeshProUGUI found!");
                return;
            }

            textMeshPro.HookupTextToSpeech();
        }

        // TODO: Better way of telling if inside hover tooltip.
        private static bool IsInvalid(Transform parent)
        {
            return parent == null;
        }
    }
}
