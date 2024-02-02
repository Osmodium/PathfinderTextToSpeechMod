using Kingmaker.UI.SettingsUI;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.VirtualListSystem.ElementSettings;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.External.ModMenu.NewTypes;

internal class UISettingsEntityImage : UISettingsEntityBase
{
    internal Sprite Sprite;
    internal int Height;
    internal float ImageScale;

    internal static UISettingsEntityImage Create(Sprite sprite, int height, float imageScale)
    {
        var image = ScriptableObject.CreateInstance<UISettingsEntityImage>();
        image.Sprite = sprite;
        image.Height = height;
        image.ImageScale = imageScale;
        return image;
    }

    public override SettingsListItemType? Type => SettingsListItemType.Custom;
}

internal class SettingsEntityImageVM : VirtualListElementVMBase
{
    internal Sprite Sprite;
    internal int Height;
    internal float ImageScale;

    internal SettingsEntityImageVM(UISettingsEntityImage imageEntity)
    {
        Sprite = imageEntity.Sprite;
        Height = imageEntity.Height;
        ImageScale = imageEntity.ImageScale;
    }

    public override void DisposeImplementation() { }
}

internal class SettingsEntityImageView : VirtualListElementViewBase<SettingsEntityImageVM>
{
    public override VirtualListLayoutElementSettings LayoutSettings
    {
        get
        {
            if (m_LayoutSettings is null)
            {
                Main.Logger.NativeLog($"Instantiating layout settings.");
                m_LayoutSettings = new()
                {
                    // For some reason it breaks if this isn't set. It doesn't work if you set the height without
                    // LayoutOverrideType.Custom, but if this is false things are no good.
                    OverrideHeight = true,
                };
                SettingsEntityPatches.OverrideType.SetValue(
                    m_LayoutSettings, VirtualListLayoutElementSettings.LayoutOverrideType.UnityLayout);
            }
            return m_LayoutSettings;
        }
    }

    private void SetHeight(float height)
    {
        Main.Logger.NativeLog($"Setting layout height: {height}");
        m_LayoutSettings = new()
        {
            OverrideHeight = true,
            Height = height,
        };

        // Without setting to custom the height is ignored.
        SettingsEntityPatches.OverrideType.SetValue(
            m_LayoutSettings, VirtualListLayoutElementSettings.LayoutOverrideType.Custom);
    }

    private VirtualListLayoutElementSettings m_LayoutSettings;

    public Image Icon;
    public GameObject TopBorder;

    public override void BindViewImplementation()
    {
        Icon.sprite = ViewModel.Sprite;

        var spriteHeight = Icon.sprite.bounds.size.y * Icon.sprite.pixelsPerUnit;

        // You can't set height to pixels directly so instead you have to scale.
        float height, scaling;
        if (ViewModel.Height > 0)
        {
            height = ViewModel.Height;
            scaling = ViewModel.Height / spriteHeight;
            scaling *= ViewModel.ImageScale;
        }
        else
        {
            height = spriteHeight;
            scaling = 1;
        }

        // The height of the row is determined by the vertical height of the sprite, regardless of its scaling. To
        // prevent the row from being too-tall, scale the height of everything.
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, scaling);
        // Just scaling the height of the image would break the aspect ratio, so scale its width.
        Icon.transform.localScale = new Vector3(scaling, Icon.transform.localScale.y);

        // Height scaling on the top bar changes its thickness, so invert it to counteract the row scaling.
        float inverseScaling = 1 / scaling;
        TopBorder.transform.localScale = new Vector3(TopBorder.transform.localScale.x, inverseScaling);

        SetHeight(height);
    }

    public override void DestroyViewImplementation() { }
}