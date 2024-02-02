using Kingmaker.Localization;
using Kingmaker.UI.SettingsUI;
using Owlcat.Runtime.UI.VirtualListSystem.ElementSettings;
using UnityEngine;

namespace SpeechMod.External.ModMenu.NewTypes;

internal class UISettingsEntitySubHeader : UISettingsEntityBase
{
    internal LocalizedString Title;
    internal bool Expanded;

    internal static UISettingsEntitySubHeader Create(LocalizedString title, bool expanded)
    {
        var subHeader = ScriptableObject.CreateInstance<UISettingsEntitySubHeader>();
        subHeader.Title = title;
        subHeader.Expanded = expanded;
        return subHeader;
    }

    public override SettingsListItemType? Type => SettingsListItemType.Custom;
}

internal class SettingsEntitySubHeaderVM : SettingsEntityCollapsibleHeaderVM
{
    public SettingsEntitySubHeaderVM(UISettingsEntitySubHeader headerEntity) : base(headerEntity.Title, headerEntity.Expanded) { }
}

internal class SettingsEntitySubHeaderView : SettingsEntityCollapsibleHeaderView
{
    public override VirtualListLayoutElementSettings LayoutSettings
    {
        get
        {
            bool set_mOverrideType = m_LayoutSettings == null;
            m_LayoutSettings ??=
                new()
                {
                    Height = 45,
                    OverrideHeight = true,
                };
            if (set_mOverrideType)
            {
                SettingsEntityPatches.OverrideType.SetValue(
                    m_LayoutSettings, VirtualListLayoutElementSettings.LayoutOverrideType.Custom);
            }

            return m_LayoutSettings;
        }
    }
    private VirtualListLayoutElementSettings m_LayoutSettings;

    protected override int GetFontSize()
    {
        return 110;
    }
}