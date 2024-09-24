using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.LocalMap;
using SpeechMod.Unity.Extensions;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class LocalMapBaseView_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(LocalMapBaseView), "Initialize")]
	public static void Initialize_Postfix(LocalMapBaseView __instance)
	{
		if (!Main.Enabled)
			return;

		__instance.m_Title.HookupTextToSpeech();
		__instance.m_LocationTypeText.HookupTextToSpeech();
	}
}