using Kingmaker.UI.SettingsUI;
using System;

namespace SpeechMod.Configuration.Settings;

public abstract class ModSettingEntry
{
    public readonly string Key;
    public readonly string Title;
    public readonly string Tooltip;

    public SettingStatus Status { get; private set; } = SettingStatus.NotApplied;

    protected ModSettingEntry(string key, string title, string tooltip)
    {
        Key = key;
        Title = title;
        Tooltip = tooltip;
    }

    public abstract SettingStatus TryEnable();

    public abstract void BuildUIAndLink();

    public abstract UISettingsEntityBase GetUISettings();

    protected SettingStatus TryPatchInternal(params Type[] type)
    {
        if (Status != SettingStatus.NotApplied) return Status;
        try
        {
            foreach (var t in type)
            {
                ModConfigurationManager.Instance?.HarmonyInstance?.CreateClassProcessor(t)?.Patch();
            }
            Status = SettingStatus.Working;
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} patch succeeded");
        }
        catch (Exception ex)
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Error($"{Title} patch exception: {ex.Message}");
            Status = SettingStatus.Error;
        }
        return Status;
    }
}
