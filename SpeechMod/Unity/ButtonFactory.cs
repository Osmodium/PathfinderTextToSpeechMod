using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using SpeechMod.Unity.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SpeechMod.Unity;

public static class ButtonFactory
{
    public const string DIALOG_ANSWER_BUTTON_NAME = "SpeechMod_DialogAnswerButton";

    private const string ARROW_BUTTON_PATH = "NestedCanvas1/DialogPCView/Body/View/Scroll View/ButtonEdge";
    private const string ARROW_BUTTON_MAP_PATH = "CombatLog_New/Panel/ButtonEdge";
    private const string MIRROR_STATIC_CANVAS_PATH = "BookEventView/ContentWrapper/Window/Mirror/Mirror";

    public static GameObject CreatePlayButton(Transform parent, UnityAction call)
    {
        return CreatePlayButton(parent, call, null, null);
    }

    private static GameObject CreatePlayButton(Transform parent, UnityAction call, string text, string toolTip)
    {
        GameObject arrowButton;

        if (UIUtility.IsGlobalMap())
        {
            arrowButton = UIHelper.TryFindInStaticCanvas(ARROW_BUTTON_MAP_PATH)?.gameObject;
            var mirror = UIHelper.TryFindInStaticCanvas(MIRROR_STATIC_CANVAS_PATH);
            if (mirror != null)
            {
                var image = mirror.GetComponent<Image>();
                if (image != null)
                {
                    image.raycastTarget = false;
                }
                else
                {
                    Debug.LogWarning("Image component not found in Mirror!");
                }
            }
            else
            {
                Debug.LogWarning("Mirror not found in GlobalMap Static Canvas!");
            }
        }
        else
        {
            arrowButton = UIHelper.TryFindInStaticCanvas(ARROW_BUTTON_PATH)?.gameObject;
        }


        if (arrowButton == null)
            return null;

        var buttonGameObject = Object.Instantiate(arrowButton, parent);
        SetupOwlcatButton(buttonGameObject, call, text, toolTip);

        return buttonGameObject;
    }

    private static void SetupOwlcatButton(GameObject buttonGameObject, UnityAction call, string text, string toolTip)
    {
        var button = buttonGameObject.GetComponent<OwlcatButton>();
        button.OnLeftClick.RemoveAllListeners();

        if (button.OnLeftClick.GetPersistentEventCount() > 0)
            button.OnLeftClick.SetPersistentListenerState(0, UnityEventCallState.Off);

        button.OnLeftClick.AddListener(call);

        if (!string.IsNullOrWhiteSpace(text))
            button.SetTooltip(new TooltipTemplateSimple(text, toolTip));
    }
}
