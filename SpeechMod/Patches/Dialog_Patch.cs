using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._VM.Dialog.Dialog;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

/// <summary>
/// Handles autoplay TTS from dialog.
/// </summary>
[HarmonyPatch(typeof(DialogVM), "HandleOnCueShow")]
public static class Dialog_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(DialogVM)}_HandleOnCueShow_Postfix");
#endif

        if (!Main.Settings.AutoPlay)
        {
#if DEBUG
            Debug.Log($"{nameof(DialogVM)}: AutoPlay is disabled!");
#endif
            return;
        }

        string key = Game.Instance?.DialogController?.CurrentCue?.Text?.Key;
        if (string.IsNullOrWhiteSpace(key))
            key = Game.Instance?.DialogController?.CurrentCue?.Text?.Shared?.String?.Key;

        if (string.IsNullOrWhiteSpace(key))
            return;

        // Stop playing and don't play if the dialog is voice acted.
        if (!Main.Settings.AutoPlayIgnoreVoice &&
            !string.IsNullOrWhiteSpace(LocalizationManager.SoundPack?.GetText(key, false)))
        {
            Main.Speech.Stop();
            return;
        }

        Main.Speech.SpeakDialog(Game.Instance?.DialogController?.CurrentCue?.DisplayText, 0.5f);
    }
}