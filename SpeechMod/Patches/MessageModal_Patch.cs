using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Common.MessageModal;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(MessageModalPCView), "BindViewImplementation")]
public class MessageModal_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(MessageModalPCView)}_BindViewImplementation_Postfix");
#endif

        var labelMessage = UIHelper.TryFindInFadeCanvas("MessageModalPCView/WindowContainer/Layout/Label_Message");
        if (labelMessage == null)
        {
            Debug.Log("Label_Message not found!");
            return;
        }

        labelMessage.HookupTextToSpeechOnTransform();
    }
}