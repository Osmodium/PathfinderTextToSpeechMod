using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using UnityEngine;

namespace SpeechMod.Configuration.UI;

public static class OwlcatUITools
{
    public static UISettingsGroup MakeSettingsGroup(string key, string name, params UISettingsEntityBase[] settings)
    {
        var group = ScriptableObject.CreateInstance<UISettingsGroup>();
        group.name = key;
        group.Title = ModLocalizationManager.CreateString(key, name);

        group.SettingsList = settings;

        return group;
    }
}
