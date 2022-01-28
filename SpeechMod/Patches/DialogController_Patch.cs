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

            // Don't stop voice if the dialog is closing or if it is not a normal dialog
            // However the playback will stop if a normal dialog is instigated while a non dialog playback is playing.
            // This is the same behaviour as in the old Infinity Engine games.
            if (Game.Instance.DialogController.Dialog.GetExitAnswer().Equals(answer) ||
                Game.Instance.DialogController.Dialog.Type != DialogType.Common)
                return;

            Main.Speech.Stop();
        }
    }
}
