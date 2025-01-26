using Kingmaker.Settings;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using System;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModToggleSettingEntry : ModSettingEntry
{
    public readonly SettingsEntityBool SettingEntity;
    public UISettingsEntityBool UiSettingEntity { get; private set; }

    protected ModToggleSettingEntry(string key, string title, string tooltip, bool defaultValue) : base(key, title, tooltip)
    {
        SettingEntity = new($"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.{Key}", defaultValue, false, true);
    }

    public override UISettingsEntityBase GetUISettings() => UiSettingEntity;

    public override void BuildUIAndLink()
    {
        UiSettingEntity = ScriptableObject.CreateInstance<UISettingsEntityBool>();
        UiSettingEntity.m_Description = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.description", Title);
        UiSettingEntity.m_TooltipDescription = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.tooltip-description", Tooltip);
        UiSettingEntity.DefaultValue = false;
        UiSettingEntity.LinkSetting(SettingEntity);
        (SettingEntity as IReadOnlySettingEntity<bool>).OnValueChanged += delegate
        {
            TryEnable();
        };
    }

    protected SettingStatus TryEnableAndPatch(Type type)
    {
        var currentValue = SettingEntity.GetValue();
        if (currentValue)
        {
            return TryPatchInternal(type);
        }
        else
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} disabled, setting integration skipped");
        }
        return SettingStatus.NOT_APPLIED;
    }
}
