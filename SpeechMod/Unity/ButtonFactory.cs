using Kingmaker;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Tooltips;
using SpeechMod.Unity.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
    private static GameObject m_ButtonPrefab = null;

    private static GameObject ArrowButton => UIHelper.TryFindInStaticCanvas(Constants.ARROW_BUTTON_PATH)?.gameObject;

    public static GameObject CreatePlayButton(Transform parent, UnityAction call)
    {
        return CreatePlayButton(parent, call, null, null);
    }

    public static void SetAction(this GameObject buttonGameObject, UnityAction call, string text = null, string toolTip = null)
    {
        var button = buttonGameObject.GetComponent<OwlcatButton>();
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

    //public static void AddOwlcatButton(string text, string tooltip, ButtonSprites sprites, UnityAction act)
    //{
    //    //var applyBuffsButton = GameObject.Instantiate(prefab, buttonsContainer.transform);
    //    //applyBuffsButton.SetActive(true);
    //    OwlcatButton button = new OwlcatButton(); //= applyBuffsButton.GetComponentInChildren<OwlcatButton>();
    //    button.m_CommonLayer[0].SpriteState = new SpriteState
    //    {
    //        pressedSprite = sprites.down,
    //        highlightedSprite = sprites.hover,
    //    };
    //    button.OnLeftClick.AddListener(() => {
    //        act();
    //    });
        
    //    //applyBuffsButton.GetComponentInChildren<Image>().sprite = sprites.normal;
    //}

    public class ButtonSprites
    {
        public Sprite normal;
        public Sprite hover;
        public Sprite down;

        public static ButtonSprites Load(string name, Vector2Int size)
        {
            return new ButtonSprites
            {
                normal = AssetLoader.LoadInternal("icons", $"{name}_normal.png", size),
                hover = AssetLoader.LoadInternal("icons", $"{name}_hover.png", size),
                down = AssetLoader.LoadInternal("icons", $"{name}_down.png", size),
            };
        }
    }

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

    public static GameObject CreateSquareButton()
    {
        if (m_ButtonPrefab != null)
            return Object.Instantiate(m_ButtonPrefab);

        var staticRoot = Game.Instance.UI.Canvas.transform;
        var buttonsContainer = staticRoot.TryFind("HUDLayout/IngameMenuView/ButtonsPart/Container");
        m_ButtonPrefab = buttonsContainer.GetChild(0).gameObject;
        return Object.Instantiate(m_ButtonPrefab);
    }
}
