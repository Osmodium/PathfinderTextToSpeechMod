using System.Collections.Generic;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Journal;
using Kingmaker.UI.MVVM._VM.Settings.Entities.Decorative;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem.ElementSettings;
using TMPro;

namespace SpeechMod.External.ModMenu.NewTypes;

internal class SettingsEntityCollapsibleHeaderVM : SettingsEntityHeaderVM
{
    internal readonly List<VirtualListElementVMBase> SettingsInGroup = new();

    private bool Initialized = false;
    internal bool Expanded { get; private set; }

    public SettingsEntityCollapsibleHeaderVM(string title, bool expanded = false) : base(title)
    {
        Expanded = expanded;
    }

    private void UpdateView()
    {
        if (Expanded)
            Expand();
        else
            Collapse();
    }

    internal void Collapse()
    {
        foreach (var entityVM in SettingsInGroup)
        {
            entityVM.Active.Value = false;
        }
    }

    internal void Expand()
    {
        bool expanded = true;
        foreach (var entityVM in SettingsInGroup)
        {
            if (entityVM is SettingsEntitySubHeaderVM subHeader)
            {
                subHeader.Active.Value = true;
                expanded = subHeader.Expanded;
            }
            else
                entityVM.Active.Value = expanded;
        }
    }

    internal void Init(ExpandableCollapseMultiButtonPC button)
    {
        button.SetValue(Expanded, true);
        if (Initialized)
            return;

        UpdateView();
        Initialized = true;
    }

    internal void Toggle(ExpandableCollapseMultiButtonPC button)
    {
        Expanded = !Expanded;
        button.SetValue(Expanded, true);
        UpdateView();
    }
}

internal class SettingsEntityCollapsibleHeaderView : VirtualListElementViewBase<SettingsEntityCollapsibleHeaderVM>
{
    public override VirtualListLayoutElementSettings LayoutSettings
    {
        get
        {
            bool set_mOverrideType = m_LayoutSettings == null;
            m_LayoutSettings ??=
                new()
                {
                    // This is the typical header height
                    Height = 65,
                    OverrideHeight = true,
                    Padding = new() { Top = 10 }
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

    public TextMeshProUGUI Title;
    public OwlcatMultiButton Button;
    public ExpandableCollapseMultiButtonPC ButtonPC;

    public override void BindViewImplementation()
    {
        Title.text = UIUtility.GetSaberBookFormat(ViewModel.Tittle, size: GetFontSize());
        Button.OnLeftClick.RemoveAllListeners();
        Button.OnLeftClick.AddListener(() => ViewModel.Toggle(ButtonPC));
        ViewModel.Init(ButtonPC);
    }

    protected virtual int GetFontSize() { return 140; }

    public override void DestroyViewImplementation() { }
}