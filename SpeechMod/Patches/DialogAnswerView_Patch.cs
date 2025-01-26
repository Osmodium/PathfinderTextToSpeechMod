using System.Collections;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.UI.MVVM._PCView.Dialog.Dialog;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using SpeechMod.Voice;
using System.Text.RegularExpressions;
using Owlcat.Runtime.UI.Controls.Button;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class DialogAnswerView_Patch
{

    private const string DIALOG_ANSWER_ARROW_TEXTURE_PATH = "Arrow";


    [HarmonyPatch(typeof(DialogAnswerView), nameof(DialogAnswerView.BindViewImplementation))]
    [HarmonyPostfix]
    public static void BindViewImplementation_Postfix(DialogAnswerView __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(DialogAnswerView)}_{nameof(BindViewImplementation_Postfix)}");
#endif

       TryAddDialogButton(__instance);
    }

    private static void TryAddDialogButton(DialogAnswerView instance)
    {
        var transform = instance.AnswerText?.transform;

#if DEBUG
        Debug.Log($"Adding/Removing dialog answer button on {instance.AnswerText?.name}...");
#endif

        var playButtonGameObject = transform?.Find(ButtonFactory.DIALOG_ANSWER_BUTTON_NAME)?.gameObject;

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
            if (instance.AnswerText == null)
                return;

            var text = instance.AnswerText.text;

            // Clean the text of tags
            text = Regex.Replace(text, "<(.*?)>", string.Empty);

            // Remove the number of the answer if the setting is disabled
            if (!Main.Settings.SayDialogAnswerNumber)
                text = new Regex(@"^(\d+\.)(.*)").Replace(text, "$2");

            text = text.PrepareText();

            Main.Speech.SpeakAs(text, Game.Instance?.Player.MainCharacter.Value?.Gender == Gender.Female ? VoiceType.Female : VoiceType.Male);
        });

        if (playButtonGameObject == null || playButtonGameObject.transform == null)
        {
            Debug.LogWarning("Failed to create the dialog answer button!");
            return;
        }

        if (Main.Settings?.DialogAnswerColorOnHover == true)
        {
            SetDialogAnswerColorHover(playButtonGameObject, instance);
        }

        playButtonGameObject.name = ButtonFactory.DIALOG_ANSWER_BUTTON_NAME;
        playButtonGameObject.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
        playButtonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
        playButtonGameObject.RectAlignMiddleLeft(new Vector2(-18f, -12f));
        playButtonGameObject.SetActive(true);

        //4. Remove the texture that indicates the hover state
        var arrowTextureTransform = instance.transform.TryFind(DIALOG_ANSWER_ARROW_TEXTURE_PATH);
        if (arrowTextureTransform)
        {
            Object.Destroy(arrowTextureTransform.gameObject);
        }
    }

    private static void SetDialogAnswerColorHover(GameObject playButtonGameObject, DialogAnswerView instance)
    {
        var highlight = instance?.transform.TryFind("Highlight");
        if (highlight == null)
            return;

        var highlightImage = highlight.GetComponent<Image>();
        if (highlightImage == null)
            return;

        var colorOff = highlightImage.color;

        var button = playButtonGameObject.GetComponent<OwlcatButton>();
        if (button == null)
            return;

        button.OnHover.RemoveAllListeners();
        button.OnHover.AddListener(hover =>
        {
            var colorOn = new Color(Main.Settings!.DialogAnswerHoverColorR, Main.Settings!.DialogAnswerHoverColorG, Main.Settings!.DialogAnswerHoverColorB, Main.Settings!.DialogAnswerHoverColorA);
            if (hover)
            {
                highlightImage.color = colorOn;
            }
            else
            {
                button.StartCoroutine(ResetColor(highlightImage, colorOff));
            }
        });
    }

    private static IEnumerator ResetColor(Image image, Color color)
    {
        yield return new WaitForEndOfFrame();
        image.color = color;
    }
}
