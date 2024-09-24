using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(TooltipEngine), nameof(TooltipEngine.GetBrickView))]
static class TooltipEngine_Patch
{
    public static void Postfix(ref MonoBehaviour __result)
    {
        if (!Main.Enabled)
            return;

        if (__result == null)
            return;

        // TODO: Possibly add more types, however it seems the text in those are split
        if (__result is not (
	            TooltipBrickTextView or
	            TooltipBrickEntityHeaderView or
	            TooltipBrickIconAndNameView or
	            TooltipBrickTitleView or
	            TooltipBrickItemFooterView
			))
            return;

        if (IsInvalid(__result.transform?.parent))
            return;

#if DEBUG
        Debug.Log(__result.transform.GetGameObjectPath());
#endif

        __result.gameObject.transform.HookupTextToSpeechOnTransform();
    }

    // TODO: Better way of telling if inside hover tooltip.
    private static bool IsInvalid(Transform parent)
    {
        return parent is null;
    }
}