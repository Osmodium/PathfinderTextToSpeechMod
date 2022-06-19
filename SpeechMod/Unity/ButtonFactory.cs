using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
    //private static GameObject m_ButtonPrefab = null;

    private static GameObject ArrowButton => UIHelper.TryFindInStaticCanvas(Constants.ARROW_BUTTON_PATH)?.gameObject;

    private static GameObject CloseButton => UIHelper.TryFindInFadeCanvas(Constants.OWLCAT_CLOSE_BUTTON_PATH)?.gameObject;
    

    public static GameObject CreatePlayButton(Transform parent, UnityAction call)
    {
        return CreatePlayButton(parent, call, null, null);
    }

    public static void SetAction(this GameObject buttonGameObject, UnityAction call, string text = null, string toolTip = null)
    {
        var button = buttonGameObject.GetComponent<OwlcatButton>();
        button.SetAction(call, text, toolTip);
    }

    public static void SetAction(this OwlcatButton button, UnityAction call, string text = null, string toolTip = null)
    {
        button.OnLeftClick.RemoveAllListeners();
        try
        {
            button.OnLeftClick.SetPersistentListenerState(0, UnityEventCallState.Off);
        }
        catch { }
        button.OnLeftClick.AddListener(call);

        if (!string.IsNullOrWhiteSpace(text))
            button.SetTooltip(new TooltipTemplateSimple(text, toolTip));
    }

    public static void CreateOwlcatButton(Transform parent, SpriteState sprites, UnityAction call)
    {
        var button = Object.Instantiate(CloseButton, parent);
        button.SetActive(true);
        OwlcatButton owlcatButton = button.GetComponentInChildren<OwlcatButton>();
        if (owlcatButton != null)
        {
            owlcatButton.m_CommonLayer[0].SpriteState = sprites;
            owlcatButton.SetAction(call, "Stop playback");
        }

        button.GetComponentInChildren<Image>().sprite = sprites.selectedSprite;
    }

    //public class ButtonSprites
    //{
    //    public Sprite normal;
    //    public Sprite hover;
    //    public Sprite down;

    //    public static ButtonSprites Load(string name, Vector2Int size)
    //    {
    //        return new ButtonSprites
    //        {
    //            normal = AssetLoader.LoadInternal("icons", $"{name}_normal.png", size),
    //            hover = AssetLoader.LoadInternal("icons", $"{name}_hover.png", size),
    //            down = AssetLoader.LoadInternal("icons", $"{name}_down.png", size),
    //        };
    //    }
    //}

    private static GameObject CreatePlayButton(Transform parent, UnityAction call, string text, string toolTip)
    {
        if (ArrowButton == null)
        {
#if DEBUG
                Debug.LogWarning("ArrowButton is null!");
                return null;
#endif
        }

        var buttonGameObject = Object.Instantiate(ArrowButton, parent);
        buttonGameObject.SetAction(call, text, toolTip);
        return buttonGameObject;
    }

    //public static GameObject CreateSquareButton()
    //{
    //    if (m_ButtonPrefab != null)
    //        return Object.Instantiate(m_ButtonPrefab);

    //    var staticRoot = Game.Instance.UI.Canvas.transform;
    //    var buttonsContainer = staticRoot.TryFind("HUDLayout/IngameMenuView/ButtonsPart/Container");
    //    m_ButtonPrefab = buttonsContainer.GetChild(0).gameObject;
    //    return Object.Instantiate(m_ButtonPrefab);
    //}
}
