using Kingmaker;
using Kingmaker.Tutorial;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.IngameMenu;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.Unity
{
    public class PlayBackControl : MonoBehaviour
    {
        public static void TryInstantiate()
        {
            if (Instance != null)
                return;

            var staticRoot = Game.Instance.UI.Canvas.transform;
            var hudLayout = staticRoot.Find("HUDLayout/").gameObject;
            
            var oldSpeechModStaticUI = hudLayout.transform.parent.Find("SpeechModStaticUI");
            if (oldSpeechModStaticUI != null) {
                Destroy(oldSpeechModStaticUI.gameObject);
            }

            var speechModStaticUI = Instantiate(hudLayout, hudLayout.transform.parent);
            
            speechModStaticUI.name = "SpeechModStaticUI";
            var rect = speechModStaticUI.transform as RectTransform;
            rect.anchoredPosition = new Vector2(0, 96);
            rect.SetSiblingIndex(hudLayout.transform.GetSiblingIndex() + 1);
            

            speechModStaticUI.DestroyComponents<HUDLayout>();
            speechModStaticUI.DestroyComponents<UISectionHUDController>();

            Destroy(rect.Find("CombatLog_New").gameObject);
            Destroy(rect.Find("Console_InitiativeTrackerHorizontalPC").gameObject);
            Destroy(rect.Find("IngameMenuView/CompassPart").gameObject);
            Destroy(rect.Find("IngameMenuView/ButtonsPart").gameObject);
            speechModStaticUI.ChildObject("IngameMenuView").DestroyComponents<IngameMenuPCView>();
            
            var inGameMenuView = rect.Find("IngameMenuView").gameObject;
            
            var canvasGameObject = new GameObject("SpeechModStaticUI");
            canvasGameObject.transform.SetParent(inGameMenuView.transform);
            canvasGameObject.layer = Game.Instance.UI.Canvas.gameObject.layer;
            canvasGameObject.transform.localPosition = Vector3.zero;
            canvasGameObject.gameObject.AddComponent<PlayBackControl>();
        }

        public static PlayBackControl Instance;

        private GameObject m_ImageGameObject;
        private RectTransform m_ImageRectTransform;
        private Image m_Image;

        public void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void Start()
        {
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            m_ImageGameObject = new GameObject("ProgressImage");
            m_ImageGameObject.transform.SetParent(transform);
            m_ImageGameObject.transform.localPosition = Vector3.zero;
            m_ImageGameObject.transform.localRotation = Quaternion.identity;
            m_ImageGameObject.transform.localScale = Vector3.one;

            m_ImageGameObject.layer = Game.Instance.UI.Canvas.gameObject.layer;
            m_ImageGameObject.transform.localPosition = Vector3.zero;
            m_ImageGameObject.transform.localRotation = Quaternion.identity;
            m_ImageGameObject.transform.localScale = Vector3.one;

            m_ImageRectTransform = m_ImageGameObject.AddComponent<RectTransform>();
            m_ImageRectTransform.pivot = new Vector2(0.5f, 1);
            m_ImageRectTransform.anchorMin = new Vector2(0, 1);
            m_ImageRectTransform.anchorMax = Vector2.one;
            m_ImageRectTransform.anchoredPosition = Vector2.zero;
            m_ImageRectTransform.sizeDelta = new Vector2(m_ImageRectTransform.sizeDelta.x, 5);
            
            m_Image = m_ImageGameObject.AddComponent<Image>();
            m_Image.color = Color.green;
        }

        public void LateUpdate()
        {
            //transform.localPosition = Vector3.zero;
            //m_ImageGameObject.transform.localPosition = Vector3.zero;
        }
    }
}
