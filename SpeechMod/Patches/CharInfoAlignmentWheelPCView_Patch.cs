using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo.Sections.Alignment.AlignmentWheel;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharInfoAlignmentWheelPCView_Patch
{
    [HarmonyPatch(typeof(CharInfoAlignmentWheelPCView), nameof(CharInfoAlignmentWheelPCView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(CharInfoAlignmentWheelPCView __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(CharInfoAlignmentWheelPCView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

        __instance.m_MythicLevel.HookupTextToSpeech();
    }
}