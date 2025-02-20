using System;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._VM.Dialog.Dialog;
using SpeechMod.ElevenLabs;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(DialogVM), "HandleOnCueShow")]
public static class Dialog_Patch
{
    public static void Postfix()
    {
        _ = RunPostfix();
    }

    private static async Task RunPostfix()
    {
        try
        {
            Main.WaveOutEvent?.Stop();

            var key = Game.Instance?.DialogController?.CurrentCue.Text.Key;
            var speaker = Game.Instance?.DialogController?.CurrentSpeakerName;
            var gender = Game.Instance?.DialogController?.CurrentSpeaker?.Gender ?? Gender.Female;

            var fileExists = false;
            var fullPath = "";

            if (!string.IsNullOrWhiteSpace(LocalizationManager.SoundPack?.GetText(key, false)))
                return;

            await VoicePlayer.PlayText(Game.Instance?.DialogController?.CurrentCue?.DisplayText, key, gender, speaker);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("Failed?");
        }
    }
}