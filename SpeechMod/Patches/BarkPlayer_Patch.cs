using System;
using HarmonyLib;
using Kingmaker;
using Kingmaker.GameModes;
using SpeechMod.Voice;
using System.Collections.Generic;
using System.Reflection;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._VM.Party;
using Kingmaker.UI.MVVM._VM.Subtitle;
using Kingmaker.UI._ConsoleUI.Overtips;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Abilities.Components;
using System.Runtime.InteropServices;
using Kingmaker.UI.Common;
using Kingmaker.UI.Overtip;
using Kingmaker.Utility;


#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class BarkPlayer_Patch
{
    [HarmonyTargetMethods]
	public static IEnumerable<MethodBase> TargetMethods()
	{
        Main.Logger.Log("BarkPlayer_TargetMethods");
		var methodName = "ShowBark";
        return
        [
            typeof(PartyVM).GetMethod(methodName),
            typeof(SubtitleVM).GetMethod(methodName),
            typeof(OvertipsVM).GetMethod(methodName)
        ];
    }

    [HarmonyPostfix]
    public static void ShowBark(MethodBase __originalMethod, IBarkHandle __result, EntityDataBase entity, string text, float duration, VoiceOverStatus voiceOverStatus = null)
    {

#if DEBUG
	    Debug.Log($"{__originalMethod.DeclaringType?.Name}_ShowBark_Postfix");
#endif

	    Debug.Log($"ResultType: {__result.GetType().Name}");
        //var entityOvertipVM = (EntityOvertipVM)__result;
        //Debug.Log($"InteractUnit: {entityOvertipVM}");
  //      foreach (var interaction in entityOvertipVM.CombineEntries)
  //      {
  //	Debug.Log($"Action {interaction.Action}");
  //	Debug.Log($"Condition {interaction.Condition}");
  //	Debug.Log($"Hint {interaction.Hint}");
  //}

        //Debug.Log($"ForceOnScreen: {((EntityOvertipVM)__result).ForceHotKeyPressed.Value}");
        //DumpProperties(__result);

        if (!BarkExtensions.ShouldPlayBark())
			return;

		BarkExtensions.DoBark(entity, text, voiceOverStatus);
    }

	public static void DumpProperties(object obj)
	{
		if (obj == null)
		{
			Debug.Log("Object is null");
			return;
		}

		var type = obj.GetType();
		var properties = type.GetProperties( BindingFlags.Public | BindingFlags.Instance);

		foreach (var property in properties)
		{
			try
			{
				var value = property.GetValue(obj, null);
				Debug.Log($"{property.Name}: {value}");
			}
			catch (Exception ex)
			{
				Debug.Log($"{property.Name}: Error accessing value - {ex.Message}");
			}
		}
	}
}

public static class BarkExtensions
{
    public static bool ShouldPlayBark()
    {
        if (!Main.Enabled)
            return false;

        if (!Main.Settings!.PlaybackBarks)
            return false;

        // Don't play barks if we are in a dialog.
        if (Game.Instance == null || Game.Instance.IsModeActive(GameModeType.Dialog))
            return false;

        switch (Main.Settings.PlaybackBarkOnlyIfSilence)
        {
            case true when Main.Speech?.IsSpeaking() == true:
            case true when Game.Instance.DialogController?.CurrentCue != null:
                return false;
        }

        if (Main.Settings.PlaybackBarksInVicinity && Main.Settings.PlaybackBarksPartyBanter)
	        return true;

        var stackTrace = Environment.StackTrace;
        Debug.Log($"BarkStackTrace: {stackTrace}");
		if (!Main.Settings.PlaybackBarksInVicinity)
        {
	        if (stackTrace.Contains("Cutscenes.Commands.CommandBark"))
		        return false;
        }

        if (!Main.Settings.PlaybackBarksPartyBanter)
        {
            if (stackTrace.Contains("BarkBanters.BarkBanterPlayer"))
                return false;
        }

        return true;
    }

    public static void DoBark(EntityDataBase entity, string text, VoiceOverStatus voiceOverStatus)
    {
	    if (voiceOverStatus != null)
		    return;

	    SpeakBark(entity, text);
    }

	private static void SpeakBark(EntityDataBase entity, string text)
    {
	    Gender? gender = null;
        if (entity is UnitEntityData unitEntityData)
            gender = unitEntityData.Gender;

#if DEBUG
        Debug.LogFormat("SpeakBark as {0}: {1}", gender.HasValue ? gender : "Narrator", text);
#endif
        switch (gender)
        {
            case Gender.Male:
                Main.Speech?.SpeakAs(text, VoiceType.Male);
                break;
            case Gender.Female:
                Main.Speech?.SpeakAs(text, VoiceType.Female);
                break;
            default:
                Main.Speech?.SpeakAs(text, VoiceType.Narrator);
                break;
        }
    }
}
