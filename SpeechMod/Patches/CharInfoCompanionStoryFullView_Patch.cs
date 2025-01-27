using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo.Sections.Stories;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharInfoCompanionStoryFullView_Patch
{
    [HarmonyPatch(typeof(CharInfoCompanionStoryFullView), nameof(CharInfoCompanionStoryFullView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(CharInfoCompanionStoryFullView __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(CharInfoCompanionStoryFullView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

        __instance.m_Title.HookupTextToSpeech();
        __instance.m_Description.HookupTextToSpeech();
    }
}