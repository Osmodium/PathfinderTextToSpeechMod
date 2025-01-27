using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.LoadingScreen;
using SpeechMod.Unity.Extensions;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class LoadingScreenPCView_Patch
{
    [HarmonyPatch(typeof(LoadingScreenPCView), nameof(LoadingScreenPCView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(LoadingScreenPCView __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(LoadingScreenPCView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

        __instance.m_CharacterDescriptionText.HookupTextToSpeech();
        __instance.m_CharacterNameText.HookupTextToSpeech();
        __instance.m_Hint.HookupTextToSpeech();
    }
}