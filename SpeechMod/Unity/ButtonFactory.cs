using Kingmaker;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.Events;

namespace SpeechMod.Unity
{
    public static class ButtonFactory
    {
        private static GameObject ArrowButton => Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View/ButtonEdge").gameObject;

        public static GameObject CreatePlayButton(Transform parent, UnityAction call)
        {
            return CreatePlayButton(parent, call, null, null);
        }

        public static GameObject CreatePlayButton(Transform parent, UnityAction call, string text, string toolTip)
        {
            var buttonGameObject = GameObject.Instantiate(ArrowButton, parent);
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
                button.SetTooltip(new TooltipTemplateSimple(text, toolTip), new TooltipConfig { 
                    InfoCallMethod = InfoCallMethod.None
                });
        }
    }
}
