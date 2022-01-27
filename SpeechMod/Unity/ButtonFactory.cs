using Kingmaker;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.Events;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
    private static GameObject m_ButtonPrefab = null;

    private static GameObject ArrowButton => UIHelper.TryFindInCanvas(Constants.ARROW_BUTTON_PATH)?.gameObject;

    public static GameObject CreatePlayButton(Transform parent, UnityAction call)
    {
        return CreatePlayButton(parent, call, null, null);
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
        SetAction(buttonGameObject, call, text, toolTip);
        return buttonGameObject;
    }

    private static void SetAction(GameObject buttonGameObject, UnityAction call, string text, string toolTip)
    {
        var button = buttonGameObject.GetComponent<OwlcatButton>();
        button.OnLeftClick.RemoveAllListeners();
        button.OnLeftClick.SetPersistentListenerState(0, UnityEventCallState.Off);
        button.OnLeftClick.AddListener(call);

        if (!string.IsNullOrWhiteSpace(text))
            button.SetTooltip(new TooltipTemplateSimple(text, toolTip), new TooltipConfig
            {
                InfoCallMethod = InfoCallMethod.None
            });
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
