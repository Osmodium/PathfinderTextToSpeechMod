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
				TooltipBrickPortraitAndNameView
			))
			return;

#if DEBUG
		Debug.Log(__result.transform.GetGameObjectPath());
#endif

		__result.gameObject.transform.HookupTextToSpeechOnTransform();
	}
}