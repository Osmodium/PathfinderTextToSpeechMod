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
            var id = Game.Instance?.DialogController?.CurrentCue.Text.Key;
            var speaker = Game.Instance?.DialogController?.CurrentSpeakerName;
            var gender = Game.Instance?.DialogController?.CurrentSpeaker?.Gender ?? Gender.Female;

            var fileExists = false;
            var fullPath = "";

            if (!string.IsNullOrWhiteSpace(LocalizationManager.SoundPack?.GetText(id, false)))
                return;

            try
            {
                fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), $@"{Main.VoiceSettings.AudioSavePath}\{id}.mp3");
                fileExists = System.IO.File.Exists(fullPath);
            }
            catch
            {
                // ignored
            }

            if (fileExists)
            {
                await VoicePlayer.PlayExistingMp3(fullPath);
            }
            else
            {
                var (finalName, textForVoice) = TextParser.MakeTextForVoice(Game.Instance?.DialogController?.CurrentCue?.DisplayText, speaker);
                var voice = Main.VoiceSettings.GetVoice(finalName, gender);
                ElevenReq req = new()
                {
                    ModelID = Main.VoiceSettings.Model,
                    Text = textForVoice,
                    Voice = voice
                };
       
                var stream = await ElevenLabsGateway.CreateStream(req);

                if (stream != null)
                    await VoicePlayer.PlayStream(stream, id);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log("Failed?");
        }
    }
}