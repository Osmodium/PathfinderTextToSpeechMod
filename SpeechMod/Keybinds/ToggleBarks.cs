using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI.MVVM._PCView.Common;
using SpeechMod.Configuration.Settings;
using System;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Keybinds;

public class ToggleBarks() : ModHotkeySettingEntry(_key, _title, _tooltip, _defaultValue)
{
    private const string _key = "barks.toggle";
    private const string _title = "Toggle barks";
    private const string _tooltip = "Toggles playback of barks on and off.";
    private const string _defaultValue = "%B;;All;false";
    private const string BIND_NAME = $"{Constants.SETTINGS_PREFIX}.newcontrols.ui.{_key}";

    public override SettingStatus TryEnable() => TryEnableAndPatch(typeof(Patches));

    [HarmonyPatch]
    private static class Patches
    {
        private static string _ToggleBarksText = "SpeechMod: Barks toggled {0}!";
        private static IDisposable _disposableBinding;

        [HarmonyPatch(typeof(CommonPCView), nameof(CommonPCView.BindViewImplementation))]
        [HarmonyPostfix]
        private static void Add(CommonPCView __instance)
        {
#if DEBUG
            Debug.Log($"{nameof(CommonPCView)}_{nameof(CommonPCView.BindViewImplementation)}_Postfix : {BIND_NAME}");
#endif
            var text = LocalizationManager.CurrentPack!.GetText("osmodium.speechmod.feature.barks.toggle.notification", false);
            if (string.IsNullOrWhiteSpace(text))
                _ToggleBarksText = text;


            if (Game.Instance.Keyboard.m_Bindings.Exists(binding => binding.Name.Equals(BIND_NAME)))
            {
#if DEBUG
                Debug.Log($"Binding {BIND_NAME} already exists! Disposing of binding...");
#endif
                _disposableBinding.Dispose();
            }

            _disposableBinding = Game.Instance!.Keyboard!.Bind(BIND_NAME, delegate { ToggleBarks(__instance); });
            __instance?.AddDisposable(_disposableBinding);
        }

        private static void ToggleBarks(CommonPCView instance)
        {
            Main.Settings.PlaybackBarks = !Main.Settings.PlaybackBarks;

            if (instance.m_WarningsText != null && Main.Settings!.ShowNotificationOnPlaybackStop)
            {
                instance.m_WarningsText?.Show(string.Format(_ToggleBarksText, Main.Settings.PlaybackBarks ? "ON" : "OFF"));
            }
        }
    }
}