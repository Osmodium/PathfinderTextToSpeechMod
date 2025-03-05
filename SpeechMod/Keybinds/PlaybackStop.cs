using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._PCView.Common;
using SpeechMod.Configuration.Settings;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Keybinds;

public class PlaybackStop : ModHotkeySettingEntry
{
    private const string _key = "playback.stop";
    private const string _title = "Stop playback";
    private const string _tooltip = "Stops playback of SpeechMod TTS.";
    private const string _defaultValue = "%S;;All;false";
    private const string BIND_NAME = $"{Constants.SETTINGS_PREFIX}.newcontrols.ui.{_key}";

    public PlaybackStop() : base(_key, _title, _tooltip, _defaultValue)
    { }

    public override SettingStatus TryEnable() => TryEnableAndPatch(typeof(Patches));

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(CommonPCView), nameof(CommonPCView.BindViewImplementation))]
        [HarmonyPostfix]
        private static void Add(CommonPCView __instance)
        {
#if DEBUG
            Debug.Log($"{nameof(CommonPCView)}_{nameof(CommonPCView.BindViewImplementation)}_Postfix : {BIND_NAME}");
#endif
            __instance?.AddDisposable(Game.Instance!.Keyboard!.Bind(BIND_NAME, delegate { StopPlayback(__instance); }));
        }

        private static void StopPlayback(CommonPCView instance)
        {
            if (!Main.Speech?.IsSpeaking() == true)
                return;

            if (instance.m_WarningsText != null)
            {
                var text = LocalizationManager.CurrentPack!.GetText("osmodium.speechmod.feature.playback.stop.notification", false);

                if (string.IsNullOrEmpty(text))
                    text = "SpeechMod: Playback stopped!";

                if (Main.Settings!.ShowNotificationOnPlaybackStop)
                    instance.m_WarningsText?.Show(text);
            }

            Main.Speech?.Stop();
        }
    }
}