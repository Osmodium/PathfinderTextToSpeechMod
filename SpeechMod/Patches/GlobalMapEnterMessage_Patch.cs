using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.GlobalMap.Message;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(GlobalMapEnterMessagePCView), "BindViewImplementation")]
public class GlobalMapEnterMessage_Patch
{
    public static void Postfix()
    {
        if (!Main.VoiceSettings.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(GlobalMapEnterMessagePCView)}_BindViewImplementation_Postfix");
#endif

        var labelMessage = UIHelper.TryFindInStaticCanvas("GlobalMapMessageBoxView/Window/Layout/Label_Message");
        if (labelMessage == null)
        {
            Debug.Log("Label_Message not found!");
            return;
        }

        labelMessage.HookupTextToSpeechOnTransform();
    }
}