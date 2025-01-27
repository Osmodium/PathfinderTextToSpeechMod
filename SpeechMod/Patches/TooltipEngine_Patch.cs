using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
static class TooltipEngine_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TooltipEngine), nameof(TooltipEngine.GetBrickView))]
    public static void GetBrickView_Postfix(ref MonoBehaviour __result)
    {
        if (!Main.Enabled)
            return;

        if (__result == null)
            return;

#if DEBUG
        Debug.Log($"{nameof(TooltipEngine)}{nameof(TooltipEngine.GetBrickView)}:{__result.GetType().Name} @ {__result.transform.GetGameObjectPath()}");
#endif

        // TODO: Possibly add more types, however it seems the text in those are split
        if (__result is not (
                TooltipBrickTextView or
                TooltipBrickEntityHeaderView or
                TooltipBrickIconAndNameView or
                TooltipBrickTitleView or
                TooltipBrickItemFooterView or
                TooltipBrickIconValueStatView or
                TooltipBrickValueStatFormulaView or
                TooltipBrickTimerView or
                TooltipBrickPortraitAndNameView or
                TooltipBrickShortClassDescriptionView or
                TooltipBrickFeatureShortDescriptionView
            ))
            return;

#if DEBUG
        Debug.Log($"{nameof(TooltipEngine)}{nameof(TooltipEngine.GetBrickView)}:{__result.transform.GetGameObjectPath()}");
#endif

        __result.gameObject.transform.HookupTextToSpeechOnTransform();
    }
}
