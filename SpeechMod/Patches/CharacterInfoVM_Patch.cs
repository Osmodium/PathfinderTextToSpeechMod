using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo.Sections.Stories;
using Kingmaker.UI.MVVM._VM.ServiceWindows.CharacterInfo;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(CharacterInfoVM), "OnPageSelected")]
public static class CharacterInfoVM_Patch
{
    public static void Postfix(CharInfoPageType pageType)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(CharacterInfoVM)}_OnPageSelected_Postfix - {pageType}");
#endif

        if (pageType is not (CharInfoPageType.SummaryPC or CharInfoPageType.BiographyPC))
            return;

        var stories = UIHelper.GetUICanvas()?.GetComponentsInChildren<CharInfoStoriesPCView>();

        for (int i = 0; i < stories.Length; ++i)
        {
            var story = stories[i];
            var textBox = story?.transform?.Find("StoryFull/StoryContent/TextBox");
            if (textBox == null)
            {
                Debug.LogWarning($"{nameof(CharacterInfoVM)}_OnPageSelected_Postfix - {nameof(textBox)} not found for pagetype {pageType}!");
                continue;
            }

            textBox.HookupTextToSpeechOnTransform();
        }
    }
}