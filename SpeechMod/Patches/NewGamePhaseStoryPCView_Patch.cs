using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.NewGame.Story;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class NewGamePhaseStoryPCView_Patch
{
    [HarmonyPatch(typeof(NewGamePhaseStoryPCView), nameof(NewGamePhaseStoryPCView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(NewGamePhaseStoryPCView __instance)
    {
        if (!Main.Enabled)
            return;
#if DEBUG
        Debug.Log($"{nameof(NewGamePhaseStoryPCView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

        __instance.m_Description.HookupTextToSpeech();
        __instance.m_DlcRequiredText.HookupTextToSpeech();
        __instance.m_LastAzlantiText.HookupTextToSpeech();
        //__instance.m_SelectorStoryText.HookupTextToSpeech();
    }
}