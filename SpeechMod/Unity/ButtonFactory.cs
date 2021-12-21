using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.IngameMenu;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpeechMod.Unity
{
    public static class ButtonFactory
    {
        private static GameObject m_ButtonPrefab = null;

        private static GameObject ArrowButton => Game.Instance.UI.Canvas.transform.TryFind("DialogPCView/Body/View/Scroll View/ButtonEdge").gameObject;

        public static GameObject CreatePlayButton(Transform parent, UnityAction call)
        {
            return CreatePlayButton(parent, call, null, null);
        }

        private static GameObject CreatePlayButton(Transform parent, UnityAction call, string text, string toolTip)
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

        public static GameObject CreateSquareButton()
        {
            if (m_ButtonPrefab != null)
                return GameObject.Instantiate(m_ButtonPrefab);

            var staticRoot = Game.Instance.UI.Canvas.transform;
            var hudLayout = staticRoot.Find("HUDLayout/");
            

            var buttonPanelRect = hudLayout.Find("IngameMenuView/ButtonsPart");
            
            var buttonsContainer = buttonPanelRect.Find("Container").gameObject;
            var buttonsRect = buttonsContainer.transform as RectTransform;
            //buttonsRect.anchoredPosition = Vector2.zero;
            //buttonsRect.sizeDelta = new Vector2(47.7f * 8, buttonsRect.sizeDelta.y);

            buttonsContainer.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.LowerLeft;

            m_ButtonPrefab = buttonsContainer.transform.GetChild(0).gameObject;
            m_ButtonPrefab.SetActive(false);
            return GameObject.Instantiate(m_ButtonPrefab);
            //prefab.SetActive(false);

//            int toRemove = buttonsContainer.transform.childCount;

////Loop from 1 and destroy child[1] since we want to keep child[0] as our prefab, which is super hacky but.
//            for (int i = 1; i < toRemove; i++)
//            {
//                Object.DestroyImmediate(buttonsContainer.transform.GetChild(1).gameObject);
//            }

        }
    }
}
