using System;
using Kingmaker.Localization;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.MVVM._PCView.Settings.Entities;
using Kingmaker.UI.MVVM._VM.Settings.Entities;
using Kingmaker.UI.SettingsUI;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.VirtualListSystem.ElementSettings;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SpeechMod.External.ModMenu.NewTypes;

public class UISettingsEntityDropdownButton : UISettingsEntityDropdownInt
{
    internal LocalizedString ButtonText;
    internal Action<int> OnClick;

    internal static UISettingsEntityDropdownButton Create(
        LocalizedString description, LocalizedString longDescription, LocalizedString buttonText, Action<int> onClick)
    {
        var button = ScriptableObject.CreateInstance<UISettingsEntityDropdownButton>();
        button.m_Description = description;
        button.m_TooltipDescription = longDescription;

        button.ButtonText = buttonText;
        button.OnClick = onClick;
        return button;
    }

    public override SettingsListItemType? Type => SettingsListItemType.Custom;
}

internal class SettingsEntityDropdownButtonVM : SettingsEntityDropdownVM
{
    private readonly UISettingsEntityDropdownButton buttonEntity;

    public string Text => buttonEntity.ButtonText;

    internal SettingsEntityDropdownButtonVM(UISettingsEntityDropdownButton buttonEntity) : base(buttonEntity)
    {
        this.buttonEntity = buttonEntity;
    }

    public void PerformClick(int selectedIndex)
    {
        buttonEntity.OnClick?.Invoke(selectedIndex);
    }
}

internal class SettingsEntityDropdownButtonView
    : SettingsEntityDropdownPCView, IPointerEnterHandler, IPointerExitHandler
{
    private SettingsEntityDropdownButtonVM VM => ViewModel as SettingsEntityDropdownButtonVM;

    public override VirtualListLayoutElementSettings LayoutSettings
    {
        get
        {
            bool set_mOverrideType = m_LayoutSettings == null;
            m_LayoutSettings ??= new();
            if (set_mOverrideType)
            {
                SettingsEntityPatches.OverrideType.SetValue(
                    m_LayoutSettings, VirtualListLayoutElementSettings.LayoutOverrideType.UnityLayout);
            }

            return m_LayoutSettings;
        }
    }

    private VirtualListLayoutElementSettings m_LayoutSettings;

    public override void BindViewImplementation()
    {
        base.BindViewImplementation();
        Title.text = VM.Title;
        ButtonLabel.text = VM.Text;
        Button.OnLeftClick.RemoveAllListeners();
        Button.OnLeftClick.AddListener(() =>
        {
            VM.PerformClick(VM.GetTempValue());
        });

        SetupColor(isHighlighted: false);
    }

    private Color NormalColor = Color.clear;
    private Color HighlightedColor = new(0.52f, 0.52f, 0.52f, 0.29f);

    // These must be public or they'll be null
    public Image HighlightedImage;
    public TextMeshProUGUI Title;
    public OwlcatButton Button;
    public TextMeshProUGUI ButtonLabel;

    private void SetupColor(bool isHighlighted)
    {
        if (HighlightedImage != null)
        {
            HighlightedImage.color = isHighlighted ? HighlightedColor : NormalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventBus.RaiseEvent(delegate (ISettingsDescriptionUIHandler h)
            {
                h.HandleShowSettingsDescription(ViewModel.Title, ViewModel.Description);
            },
            true);
        SetupColor(isHighlighted: true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventBus.RaiseEvent(delegate (ISettingsDescriptionUIHandler h)
            {
                h.HandleHideSettingsDescription();
            },
            true);
        SetupColor(isHighlighted: false);
    }
}