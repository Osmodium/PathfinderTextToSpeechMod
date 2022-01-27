using HarmonyLib;
using Kingmaker;
using Kingmaker.Controllers.Dialog;
using Kingmaker.DialogSystem.Blueprints;
using UnityEngine;

namespace SpeechMod.Patches
{
    [HarmonyPatch(typeof(DialogController), "SelectAnswer")]
    public class DialogController_Patch
    {
        public static void Prefix(BlueprintAnswer answer)
        {
            if (!Main.Enabled)
                return;

#if DEBUG
            Debug.Log($"{nameof(DialogController)}_SelectAnswer_Prefix");
#endif

            if (!Main.Settings.AutoPlay)
            {
#if DEBUG
                Debug.Log("Autoplay is disabled!");
#endif
                return;
            }

            // Don't stop voice if the dialog is closing.
            if (Game.Instance.DialogController.Dialog.GetExitAnswer().Equals(answer))
                return;

            Main.Speech.Stop();
        }
    }
}
