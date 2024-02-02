using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.Settings;
using Kingmaker.UI;
using Kingmaker.UI.SettingsUI;
using UnityEngine;
using static Kingmaker.UI.KeyboardAccess;

namespace SpeechMod.External.ModMenu.Settings;

public class KeyBinding
    : BaseSettingWithValue<KeyBindingPair, SettingsEntityKeyBindingPair, UISettingsEntityKeyBinding, KeyBinding>
{
    private GameModesGroup GameModesGroup;
    private string PrimaryBinding = null;
    private string SecondaryBinding = null;
    private KeyBindingPair DefaultOverride;
    private bool IsHoldTrigger = false;

    /// <inheritdoc cref="KeyBinding(string, GameModesGroup, LocalizedString)"/>
    public static KeyBinding New(string key, GameModesGroup gameModesGroup, LocalizedString description)
    {
        return new(key, gameModesGroup, description);
    }

    protected override SettingsEntityKeyBindingPair CreateEntity()
    {
        DefaultOverride = DefaultValue;
        if (!string.IsNullOrEmpty(PrimaryBinding))
        {
            var bindingString = new StringBuilder($"!{PrimaryBinding}");
            if (!string.IsNullOrEmpty(SecondaryBinding))
                bindingString.Append($";{SecondaryBinding}");

            DefaultOverride = new(bindingString.ToString(), GameModesGroup);
        }
        return new SettingsEntityKeyBindingPair(Key, DefaultOverride, SaveDependent, RebootRequired);
    }

    protected override UISettingsEntityKeyBinding CreateUIEntity()
    {
        var uiEntity = ScriptableObject.CreateInstance<UISettingsEntityKeyBinding>();
        uiEntity.name = Key;
        uiEntity.IsHoldTrigger = IsHoldTrigger;
        return uiEntity;
    }

    /// <summary>
    /// If true, the key binding is activated only when held down rather than just pressed.
    /// </summary>
    public KeyBinding SetIsHoldTrigger(bool isHoldTrigger = true)
    {
        IsHoldTrigger = isHoldTrigger;
        return this;
    }

    /// <summary>
    /// Sets the default key binding.
    /// </summary>
    /// 
    /// <param name="keyCode">Unity's key code for the binding</param>
    /// <param name="withCtrl">If true, the binding includes the Ctrl key</param>
    /// <param name="withAlt">If true, the binding includes the Alt key</param>
    /// <param name="withShift">If true, the binding includes the Shift key</param>
    public KeyBinding SetPrimaryBinding(
        KeyCode keyCode, bool withCtrl = false, bool withAlt = false, bool withShift = false)
    {
        PrimaryBinding = Create(keyCode, withCtrl, withAlt, withShift);
        return this;
    }

    /// <summary>
    /// Sets the default alternate binding. This is just a second key combination for the same binding. Ignored if
    /// there is no primary binding, see <see cref="SetPrimaryBinding(KeyCode, bool, bool, bool)"/>.
    /// </summary>
    /// 
    /// <param name="keyCode">Unity's key code for the binding</param>
    /// <param name="withCtrl">If true, the binding includes the Ctrl key</param>
    /// <param name="withAlt">If true, the binding includes the Alt key</param>
    /// <param name="withShift">If true, the binding includes the Shift key</param>
    public KeyBinding SetSecondaryBinding(
        KeyCode keyCode, bool withCtrl = false, bool withAlt = false, bool withShift = false)
    {
        SecondaryBinding = Create(keyCode, withCtrl, withAlt, withShift);
        return this;
    }

    /// <inheritdoc cref="BaseSettingWithValue{T, TEntity, TUIEntity, TBuilder}.BaseSettingWithValue(string, T, LocalizedString)"/>
    /// 
    /// <param name="gameModesGroup">Indicates in which game modes the key binding functions</param>
    public KeyBinding(string key, GameModesGroup gameModesGroup, LocalizedString description)
        : base(key, new("", gameModesGroup), description)
    {
        GameModesGroup = gameModesGroup;
    }

    private static string Create(KeyCode keyCode, bool withCtrl, bool withAlt, bool withShift)
    {
        var keyBinding = new StringBuilder();
        if (withCtrl)
            keyBinding.Append("%");
        if (withAlt)
            keyBinding.Append("&");
        if (withShift)
            keyBinding.Append("#");
        keyBinding.Append(keyCode.ToString());

        return keyBinding.ToString();
    }
}

[HarmonyPatch(typeof(KeyboardAccess))]
internal static class KeyboardAccess_Patch
{
    private static readonly List<KeyBinding> KeyBindings = new();

    internal static void RegisterBinding(
        SettingsEntityKeyBindingPair entity, UISettingsEntityKeyBinding uiEntity, Action onPress)
    {
        var keyBinding = new KeyBinding(entity, uiEntity, onPress);
        KeyBindings.Add(keyBinding);
        RegisterBinding(keyBinding);
    }

    private static void RegisterBinding(KeyBinding keyBinding)
    {
        // First register the binding, then associate it with onPress
        var binding = keyBinding.Entity.GetValue();
        if (!keyBinding.UiEntity.TrySetBinding(binding.Binding1, 0))
        {
            Main.Logger.Warning($"Unable to set binding: {keyBinding.Entity.Key} - {binding.Binding1}");
            keyBinding.Entity.SetKeyBindingDataAndConfirm(default, 0);
        }
        if (!keyBinding.UiEntity.TrySetBinding(binding.Binding2, 1))
        {
            Main.Logger.Warning($"Unable to set binding: {keyBinding.Entity.Key} - {binding.Binding2}");
            keyBinding.Entity.SetKeyBindingDataAndConfirm(default, 1);
        }

        if (Game.Instance.Keyboard.m_BindingCallbacks.TryGetValue(keyBinding.Entity.Key, out var callbacks) && callbacks.Count > 0)
        {
#if DEBUG
            Main.Logger.Log($"Callback binding found: {keyBinding.Entity.Key}");
#endif
            return;
        }

#if DEBUG
        Main.Logger.Log($"Binding callback: {keyBinding.Entity.Key}");
#endif
        Game.Instance.Keyboard.Bind(keyBinding.Entity.Key, keyBinding.OnPress);
    }

    // This patch is needed because all key bindings are cleared (and thus get disposed) every time you load to a
    // different mode (e.g. from in game back to main menu). This re-registers the settings, similar to the flow used
    // for base game key bindings.
    [HarmonyPatch(nameof(KeyboardAccess.RegisterBuiltinBindings)), HarmonyPrefix]
    static void RegisterBuiltinBindings()
    {
        try
        {
#if DEBUG
            Main.Logger.Log("Re-registering key bindings.");
#endif
            foreach (var keyBinding in KeyBindings)
            {
                keyBinding.UiEntity.RenewRegisteredBindings();
                RegisterBinding(keyBinding);
            }
        }
        catch (Exception e)
        {
            Main.Logger.LogException("KeyboardAccess_Patch.RegisterBuiltinBindings", e);
        }
    }

    private struct KeyBinding
    {
        public readonly SettingsEntityKeyBindingPair Entity;
        public readonly UISettingsEntityKeyBinding UiEntity;
        public readonly Action OnPress;

        public KeyBinding(SettingsEntityKeyBindingPair entity, UISettingsEntityKeyBinding uiEntity, Action onPress)
        {
            Entity = entity;
            UiEntity = uiEntity;
            OnPress = onPress;
        }
    }
}