using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Settings.Entities;
using Kingmaker.UI.MVVM._VM.Settings.Entities;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class SettingsEntityView_Patch
{
    [HarmonyPatch(typeof(SettingsEntityView<SettingsEntityVM>), nameof(SettingsEntityView<SettingsEntityVM>.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(SettingsEntityView<SettingsEntityVM> __instance)
    {
        if (!Main.Enabled)
            return;
#if DEBUG
        Debug.Log($"{nameof(SettingsEntityView<SettingsEntityVM>)}_{nameof(BindViewImplementation_Postfix)}");
#endif
        var textToRead = $"{__instance.ViewModel.Title}. {__instance.ViewModel.Description}";

        __instance.m_Title.HookupTextToSpeech(textToRead);
    }
}