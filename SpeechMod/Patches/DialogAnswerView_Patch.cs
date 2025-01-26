using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UI.MVVM._PCView.Dialog.Dialog;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using SpeechMod.Voice;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class DialogAnswerView_Patch
{
    private const string DIALOG_ANSWER_BUTTON_NAME = "SpeechMod_DialogAnswerButton";

    [HarmonyPatch(typeof(DialogAnswerView), nameof(DialogAnswerView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(DialogAnswerView __instance)
    {
        if (!Main.Enabled)
            return;
#if DEBUG
        Debug.Log($"{nameof(DialogAnswerView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

       TryAddDialogButton(__instance.AnswerText, new Vector2(-30f, -12f));
    }

    private static void TryAddDialogButton(TextMeshProUGUI textMeshPro, Vector2? anchoredPosition = null)
    {
        var transform = textMeshPro?.transform;

#if DEBUG
        Debug.Log($"Adding/Removing dialog answer button on {textMeshPro?.name}...");
#endif
        var playButtonGameObject = transform?.Find(DIALOG_ANSWER_BUTTON_NAME)?.gameObject;

        // 1. Check if the setting for showing the playback button is enabled.
        if (!Main.Settings.ShowPlaybackOfDialogAnswers)
        {
            // 1a. Destroy the button if it exists.
            if (playButtonGameObject != null)
                Object.Destroy(playButtonGameObject);
            return;
        }

        // 2. Check if the button already exists.
        if (playButtonGameObject != null)
            return;

        // 3. Create the button if it doesn't exist.
        playButtonGameObject = ButtonFactory.CreatePlayButton(transform, () =>
        {
            if (textMeshPro == null)
                return;
            var text = textMeshPro.text;

            text = Main.Settings?.SayDialogAnswerNumber == true ?
                new Regex("<alpha[^>]+>([^>]+)<alpha[^>]+><indent[^>]+>([^<>]*)</indent>").Replace(text, "$1 - $2") :
                new Regex("<alpha[^>]+>[^>]+<alpha[^>]+><indent[^>]+>([^<>]*)</indent>").Replace(text, "$1");

            text = text.PrepareText();

            Main.Speech.SpeakAs(text, Game.Instance?.Player.MainCharacter.Value?.Gender == Gender.Female ? VoiceType.Female : VoiceType.Male);
        });

        if (playButtonGameObject == null || playButtonGameObject.transform == null)
            return;

        playButtonGameObject.name = DIALOG_ANSWER_BUTTON_NAME;
        playButtonGameObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        playButtonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        playButtonGameObject.RectAlignMiddleLeft(anchoredPosition);
        playButtonGameObject.SetActive(true);

    }
}


// (<([^>]+)>)

// 2. <color=#2D3406><link="SkillcheckDC:CheckDiplomacy"><b>[Diplomacy 20]</b></link> </color> ...address the soldiers and boost their morale.
// Diplomacy 20 ...address the soldiers and boost their morale.
// 1. "I'd like to know more about you."
//
//1. Hover icon leaves a lot of space, and it's difficult to see when hovering the playback button
// 2. The button does not exist in WorldMap scene
// 3. Text check coloring
// 	2. <color=#2D3406><link="SkillcheckDC:CheckDiplomacy"><b>[Diplomacy 20]</b></link> </color> ...address the soldiers and boost their morale.
// 	Diplomacy 20 ...address the soldiers and boost their morale.
//
// 	1. "I'd like to know more about you."

