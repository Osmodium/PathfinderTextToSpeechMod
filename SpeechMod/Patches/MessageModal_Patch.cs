using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Common.MessageModal;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches
{
    [HarmonyPatch(typeof(MessageModalPCView), "BindViewImplementation")]
    public class MessageModal_Patch
    {
        public static void Postfix()
        {
#if DEBUG
            Debug.Log($"{nameof(MessageModalPCView)}_BindViewImplementation_Postfix");
#endif

            var labelMessage = Game.Instance.UI.FadeCanvas.transform.TryFind("MessageModalPCView/WindowContainer/Layout/Label_Message");
            if (labelMessage == null)
            {
                Debug.Log("Label_Message not found!");
                return;
            }

            labelMessage.HookupTextToSpeechOnTransform();
        }
    }
}
