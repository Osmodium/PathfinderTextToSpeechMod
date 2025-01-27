using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Settings.Entities.Difficulty;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class SettingsEntityDropdownGameDifficultyItemPCView_Patch
{
    [HarmonyPatch(typeof(SettingsEntityDropdownGameDifficultyItemPCView), nameof(SettingsEntityDropdownGameDifficultyItemPCView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(SettingsEntityDropdownGameDifficultyItemPCView __instance)
    {
        if (!Main.Enabled)
            return;
#if DEBUG
        Debug.Log($"{nameof(SettingsEntityDropdownGameDifficultyItemPCView)}_{nameof(BindViewImplementation_Postfix)}");
#endif
        var textToRead = $"{__instance.ViewModel.Title}. {__instance.ViewModel.Description}";

        __instance.m_Title.HookupTextToSpeech(textToRead);
    }
}