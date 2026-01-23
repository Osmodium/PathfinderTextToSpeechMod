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

public class ToggleAutoPlay() : ModHotkeySettingEntry(_key, _title, _tooltip, _defaultValue)
{
    private const string _key = "autoplay.toggle";
    private const string _title = "Toggle autoplay";
    private const string _tooltip = "Toggles autoplay on and off.";
    private const string _defaultValue = "%T;;All;false";
    private const string BIND_NAME = $"{Constants.SETTINGS_PREFIX}.newcontrols.ui.{_key}";

    public override SettingStatus TryEnable() => TryEnableAndPatch(typeof(Patches));

    [HarmonyPatch]
    private static class Patches
    {
        private static string _ToggleAutoPlayOffText = "SpeechMod: Autoplay toggled OFF!";
        private static string _ToggleAutoPlayOnText = "SpeechMod: Autoplay toggled ON!";
        private static IDisposable _disposableBinding;

        [HarmonyPatch(typeof(CommonPCView), nameof(CommonPCView.BindViewImplementation))]
        [HarmonyPostfix]
        private static void Add(CommonPCView __instance)
        {
#if DEBUG
            Debug.Log($"{nameof(CommonPCView)}_{nameof(CommonPCView.BindViewImplementation)}_Postfix : {BIND_NAME}");
#endif
            var autoPlayOffText = LocalizationManager.CurrentPack!.GetText("osmodium.speechmod.feature.autoplay.toggle.notification.off", false);
            if (!string.IsNullOrWhiteSpace(autoPlayOffText))
                _ToggleAutoPlayOffText = autoPlayOffText;

            var autoPlayOnText = LocalizationManager.CurrentPack!.GetText("osmodium.speechmod.feature.autoplay.toggle.notification.on", false);
            if (!string.IsNullOrWhiteSpace(autoPlayOnText))
                _ToggleAutoPlayOnText = autoPlayOnText;

            if (Game.Instance.Keyboard.m_Bindings.Exists(binding => binding.Name.Equals(BIND_NAME)))
            {
#if DEBUG
                Debug.Log($"Binding {BIND_NAME} already exists! Disposing of binding...");
#endif
                _disposableBinding.Dispose();
            }

            _disposableBinding = Game.Instance!.Keyboard!.Bind(BIND_NAME, delegate { ToggleAutoPlay(__instance); });
            __instance?.AddDisposable(_disposableBinding);
        }

        private static void ToggleAutoPlay(CommonPCView instance)
        {
            Main.Settings.AutoPlay = !Main.Settings.AutoPlay;

            if (instance.m_WarningsText != null && Main.Settings!.ShowNotificationOnPlaybackStop)
            {
                instance.m_WarningsText?.Show(Main.Settings.AutoPlay ? _ToggleAutoPlayOnText : _ToggleAutoPlayOffText);
            }
        }
    }
}