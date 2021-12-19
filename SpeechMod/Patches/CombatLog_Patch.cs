using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.Log.CombatLog_ThreadSystem;
using Kingmaker.UI.MVVM._PCView.CombatLog;
using Kingmaker.UI.MVVM._VM.CombatLog;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches
{
    //[HarmonyPatch(typeof(CombatLogVM), "SwitchVisibleLog")]
    //[HarmonyPatch(typeof(CombatLogPCView), "SetSizeDelta")]
    //[HarmonyPatch(typeof(CombatLogPCView), "OnItemsAdded")]
    //[HarmonyPatch(typeof(CombatLogPCView), "OnChannelUpdated")]
    //[HarmonyPatch(typeof(CombatLogPCView), "OnVisible")]
    //[HarmonyPatch(typeof(CombatLogPCView), "OnPointerEnterToggle")]
    
    //[HarmonyPatch(typeof(CombatLogVM), "SetCurrentChannel")]
    //public static class CombatLog_Patch_SCC
    //{
    //    public static void Postfix()
    //    {
    //        CombatLog_Patch.Postfix();
    //    }
    //}

    //[HarmonyPatch(typeof(CombatLogVM), "GetLastVisibleItemForChannel")]
    //public static class CombatLog_Patch_GLVIFC
    //{
    //    public static void Postfix()
    //    {
    //        CombatLog_Patch.Postfix();
    //    }
    //}

    //[HarmonyPatch(typeof(CombatLogVM), "AddNewMessage")]
    //public static class CombatLog_Patch_ANM
    //{
    //    public static void Postfix()
    //    {
    //        CombatLog_Patch.Postfix();
    //    }
    //}

    [HarmonyPatch(typeof(CombatLogChannel), "AddNewMessage")]
    public static class CombatLog_Patch_ANM
    {
        public static void Postfix()
        {
            CombatLog_Patch.Postfix();
        }
    }
    
    public static class CombatLog_Patch
    {
        public static void Postfix()
        {
#if DEBUG
            Debug.Log($"{nameof(CombatLogVM)}_xx_Postfix");
#endif

            GameObject content = null;
            try
            {
                content = Game.Instance?.UI?.Canvas?.transform?.Find("HUDLayout/CombatLog_New/Panel/Scroll View/Viewport/Content")?.gameObject;
            }
            catch{}

            if (content == null)
            {
                Debug.Log("CombatLog Content not found!");
                return;
            }

            var allTexts = content.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (var text in allTexts)
            {
                text.HookupTextToSpeech();
            }
        }
    }
}
