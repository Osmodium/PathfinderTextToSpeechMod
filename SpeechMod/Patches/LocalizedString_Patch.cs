using System;
using HarmonyLib;
using Kingmaker;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using UnityEngine;

namespace SpeechMod.Patches
{
	[HarmonyPatch(typeof(LocalizedString), "PlayVoiceOver")]
	public class LocalizedString_Patch
	{
		private static void Prefix(MonoBehaviour target, LocalizedString __instance, VoiceOverStatus __result)
		{
      // skip if dialog / book
			if (Game.Instance.IsModeActive(GameModeType.Dialog))
			{
				return;
			}
			if (!Main.Enabled)
			{
				return;
			}
			if (!Main.Settings.AutoPlay)
			{
				return;
			}
      // only play if no voiceover part1
			if (__result != null)
			{
				return;
			}
      // only play if no voiceover part2
			if (target == null)
			{
				return;
			}
			Game instance = Game.Instance;
			Main.Speech.Speak(__instance, 0.5f);
		}
	}
}
