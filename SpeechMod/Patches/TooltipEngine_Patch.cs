using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;

namespace SpeechMod;

[HarmonyPatch(typeof(TooltipEngine), "GetBrickView")]
static class TooltipEngine_Patch
{
    public static void Postfix(ref MonoBehaviour __result)
    {
        if (!Main.Enabled)
            return;

        if (__result == null)
            return;

        // TODO: Possibly add more types, however it seems the text in those are split
        if (!(__result is TooltipBrickTextView view))
            return;

        if (IsInvalid(view.transform?.parent))
            return;

#if DEBUG
        Debug.Log(__result.transform.GetGameObjectPath());
#endif

        var textMeshProTransform = view.gameObject?.transform?.TryFind("Text (TMP)");
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
        return parent is null;
    }
}