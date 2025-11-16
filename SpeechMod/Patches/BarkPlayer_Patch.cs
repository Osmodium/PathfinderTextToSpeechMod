using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Localization;
using Kingmaker.UI._ConsoleUI.Overtips;
using SpeechMod.Voice;

#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class BarkPlayer_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EntityOvertipVM), nameof(EntityOvertipVM.ShowBark), typeof(string), typeof(float), typeof(VoiceOverStatus))]
    public static void PlayBark_Postfix(EntityOvertipVM __instance, string text, float duration, VoiceOverStatus voiceOverStatus)
    {
        if (!Main.Enabled || !Main.Settings.PlaybackBarks)
            return;

#if DEBUG
        Debug.Log($"{nameof(EntityOvertipVM)}_{nameof(EntityOvertipVM.ShowBark)}_Postfix");
#endif

        // Don't interfere if the game is already playing a voiceover or a TTS is playing!
        var isAnyoneSpeaking = voiceOverStatus != null || Main.Speech.IsSpeaking();
        if (isAnyoneSpeaking && Main.Settings.PlaybackBarkOnlyIfSilence)
            return;

        VoiceType voice;
        if (__instance.Unit.IsMainCharacter && Main.Settings.UseProtagonistSpecificVoice)
        {
            voice = VoiceType.Protagonist;
        }
        else
        {
            Gender? gender = null;
            if (__instance.Unit != null)
                gender = __instance.Unit.Gender;

            voice = gender switch
            {
                Gender.Male => VoiceType.Male,
                Gender.Female => VoiceType.Female,
                _ => VoiceType.Narrator
            };
        }

        Main.Speech.SpeakAs(text, voice);
    }
}