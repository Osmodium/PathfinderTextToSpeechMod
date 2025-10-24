using HarmonyLib;
using Kingmaker.UI;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class TermsOfUseController_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TermsOfUseController), "Bind")]
    public static void Bind_Postfix(TermsOfUseController __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(TermsOfUseController)}_Bind_Postfix");
#endif

        __instance.m_TitleText.HookupTextToSpeech();
        __instance.m_LicenceText.HookupTextToSpeech();
        __instance.m_SubLicenceText.HookupTextToSpeech();
    }
}
