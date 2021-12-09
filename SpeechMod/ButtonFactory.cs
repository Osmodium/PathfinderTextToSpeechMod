using Kingmaker;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.Events;

namespace SpeechMod
{
    public static class ButtonFactory
    {
        private static GameObject Button
        {
            get
            {
                return Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View/ButtonEdge").gameObject;
            }
        }

        public static GameObject CreateButton()
        {
            return Object.Instantiate(Button);
        }

        public static GameObject CreateButton(Transform parent)
        {
            return Object.Instantiate(Button, parent);
        }

        public static GameObject CreatePlayButton(UnityAction call)
        {
            var buttonGameObject = CreateButton();

            SetAction(buttonGameObject, call);

            return buttonGameObject;
        }

        public static GameObject CreatePlayButton(Transform parent, UnityAction call)
        {
            var buttonGameObject = CreateButton(parent);

            SetAction(buttonGameObject, call);

            return buttonGameObject;
        }

        private static void SetAction(GameObject buttonGameObject, UnityAction call)
        {
            var button = buttonGameObject.GetComponent<OwlcatButton>();
            button.OnLeftClick.RemoveAllListeners();
            button.OnLeftClick.SetPersistentListenerState(0, UnityEventCallState.Off);
            button.OnLeftClick.AddListener(call);
        }
    }
}
