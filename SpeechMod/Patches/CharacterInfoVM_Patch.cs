using System;
using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.CharacterInfo.Sections.Stories;
using Kingmaker.UI.MVVM._VM.ServiceWindows.CharacterInfo;
using SpeechMod.Unity.Extensions;
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
        Debug.Log($"{nameof(CharacterInfoVM)}_OnPageSelected_Postfix @ {pageType}");
#endif

        if (pageType is not (CharInfoPageType.SummaryPC or CharInfoPageType.BiographyPC))
            return;

        var stories = UIHelper.GetUICanvas()?.GetComponentsInChildren<CharInfoStoriesPCView>();

        if (stories == null)
	        return;

        foreach (var story in stories)
        {
	        try
	        {
		        var textBox = story?.transform?.Find("StoryFull/StoryContent/TextBox");
		        if (textBox == null)
		        {
			        Debug.LogWarning(
				        $"{nameof(CharacterInfoVM)}_OnPageSelected_Postfix - TextBox not found for pagetype {pageType}!");
			        continue;
		        }

		        textBox.HookupTextToSpeechOnTransform();
	        }
	        catch (Exception ex)
	        {
		        Debug.LogWarning($"Failed hooking story text on story '{story.name}'. {ex.Message}");
	        }
        }
    }
}