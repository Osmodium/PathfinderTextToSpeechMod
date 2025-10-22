using HarmonyLib;
using Kingmaker.UI;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public class TermsOfUseController_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TermsOfUseController), "Bind")]
    public static void Bind_Postfix(TermsOfUseController __instance)
    {
        Debug.Log("1");
        if (!Main.Enabled)
            return;
        Debug.Log("2");
#if DEBUG
        Debug.Log($"{nameof(TermsOfUseController)}_Bind_Postfix");
#endif
        Debug.Log("3");
        __instance.m_TitleText.HookupTextToSpeech();
        Debug.Log("4");
        __instance.m_LicenceText.HookupTextToSpeech();
        Debug.Log("5");
        __instance.m_SubLicenceText.HookupTextToSpeech();
        Debug.Log("6");
    }
}
