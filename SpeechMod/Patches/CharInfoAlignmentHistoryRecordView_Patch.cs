using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo.Sections.Alignment.AlignmentHistory;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharInfoAlignmentHistoryRecordView_Patch
{
    [HarmonyPatch(typeof(CharInfoAlignmentHistoryRecordView), nameof(CharInfoAlignmentHistoryRecordView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(CharInfoAlignmentHistoryRecordView __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(CharInfoAlignmentHistoryRecordView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

        __instance.m_Description.HookupTextToSpeech();
        __instance.m_Shift.HookupTextToSpeech();
    }
}