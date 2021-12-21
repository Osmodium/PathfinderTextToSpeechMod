using Kingmaker;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using SpeechMod.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.Unity
{
    public class PlaybackControl : MonoBehaviour
    {
        private static Color m_PlaybackProgressColor = Color.green;

        public static void UpdatePlaybackProgessColor()
        {
            m_PlaybackProgressColor = new Color(Main.Settings.PlaybackColorR, Main.Settings.PlaybackColorG, Main.Settings.PlaybackColorB, Main.Settings.PlaybackColorA);
        }

        public static void TryInstantiate()
        {
            if (Instance != null)
                return;

            var staticRoot = Game.Instance.UI.Canvas.transform;
            
            var canvasGameObject = new GameObject("SpeechModStaticUI")
            {
                layer = Game.Instance.UI.Canvas.gameObject.layer
            };
            canvasGameObject.transform.SetParent(staticRoot.transform);
            canvasGameObject.transform.localPosition = Vector3.zero;
            canvasGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            canvasGameObject.transform.localScale = Vector3.one;
            canvasGameObject.gameObject.AddComponent<PlaybackControl>();
        }

        public static PlaybackControl Instance;

        private GameObject m_ImageGameObject;
        private RectTransform m_ImageRectTransform;
        private Image m_Image;
        private GameObject m_TextGameObject;
        private TextMeshProUGUI m_TextMeshProUGUI;
        private GameObject m_StopButton;

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
            rectTransform.transform.localPosition = Vector3.zero;
            rectTransform.transform.localRotation = Quaternion.Euler(Vector3.zero);
            rectTransform.transform.localScale = Vector3.one;

            rectTransform.SetSize(Vector2.zero);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = Vector2.zero;

            m_ImageGameObject = new GameObject("ProgressImage")
            {
                layer = Game.Instance.UI.Canvas.gameObject.layer
            };
            m_ImageGameObject.SetActive(false);
            m_ImageGameObject.transform.SetParent(transform);
            m_ImageGameObject.transform.localPosition = Vector3.zero;
            m_ImageGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

            m_ImageRectTransform = m_ImageGameObject.AddComponent<RectTransform>();
            m_ImageRectTransform.SetSize(Vector2.zero);
            m_ImageRectTransform.anchorMin = new Vector2(0, 1);
            m_ImageRectTransform.anchorMax = Vector2.one;
            m_ImageRectTransform.pivot = new Vector2(0, 1);
            m_ImageRectTransform.anchoredPosition = Vector2.zero;
            m_ImageRectTransform.SetHeight(5);
            m_Image = m_ImageGameObject.AddComponent<Image>();
            m_ImageGameObject.transform.localScale = new Vector3(0, 1, 1);

            m_StopButton = ButtonFactory.CreateSquareButton();
            var buttonRectTransform = m_StopButton.GetComponent<RectTransform>();
            buttonRectTransform.anchorMin = new Vector2(0, 1);
            buttonRectTransform.anchorMax = new Vector2(0, 1);
            buttonRectTransform.pivot = new Vector2(0, 1);
            m_StopButton.transform.SetParent(transform);
            m_StopButton.transform.localPosition = Vector3.zero;
            m_StopButton.transform.localRotation = Quaternion.Euler(Vector3.zero);
            m_StopButton.transform.localScale = Vector3.one;

            OwlcatButton owlCatButton = m_StopButton.GetComponentInChildren<OwlcatButton>();
            owlCatButton.OnLeftClick.AddListener(StopPlayback);
            owlCatButton.SetTooltip(new TooltipTemplateSimple("Stop TTS Playback", "Stops the current text to speech playback."), new TooltipConfig {
                InfoCallMethod = InfoCallMethod.None
            });

#if DEBUG
            m_TextGameObject = new GameObject("Text");
            m_TextGameObject.transform.SetParent(transform);
            m_TextGameObject.transform.localPosition = Vector3.zero;
            m_TextGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            m_TextGameObject.transform.localScale = Vector3.one;
            var textRectTransform = m_TextGameObject.AddComponent<RectTransform>();
            textRectTransform.anchorMin = new Vector2(0, 1);
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.pivot = new Vector2(0, 1);
            textRectTransform.anchoredPosition = Vector2.zero;

            m_TextMeshProUGUI = m_TextGameObject.AddComponent<TextMeshProUGUI>();
            m_TextMeshProUGUI.fontStyle = FontStyles.Bold;
            m_TextMeshProUGUI.text = "DEBUG Playback control";
#endif
        }

        public void LateUpdate()
        {
            if (WindowsVoiceUnity.IsSpeaking)
            {
                if (!Main.Settings.ShowPlaybackProgress)
                    return;
#if DEBUG
                if (!m_TextGameObject.activeSelf)
                    m_TextGameObject.SetActive(true);
                m_TextMeshProUGUI.text = $"{WindowsVoiceUnity.WordPosition}/{WindowsVoiceUnity.WordCount} : {WindowsVoiceUnity.GetNormalizedProgress()}";
#endif
                if (!m_ImageGameObject.activeSelf)
                    m_ImageGameObject.SetActive(true);
                m_Image.color = m_PlaybackProgressColor;
                m_ImageGameObject.transform.localScale = Vector3.Slerp(m_ImageGameObject.transform.localScale, new Vector3(WindowsVoiceUnity.GetNormalizedProgress(), 1, 1), 0.1f);

                if (!m_StopButton.activeSelf)
                    m_StopButton.SetActive(true);

                return;
            }
#if DEBUG
            if (m_TextGameObject.activeSelf)
                m_TextGameObject.SetActive(false);
#endif
            m_ImageGameObject.transform.localScale = Vector3.Slerp(m_ImageGameObject.transform.localScale, new Vector3(1, 1, 1), 0.1f);
            if (m_ImageGameObject.activeSelf)
            {
                m_ImageGameObject.SetActive(false);
                m_ImageGameObject.transform.localScale = new Vector3(0, 1, 1);
            }

            if (m_StopButton.activeSelf)
                m_StopButton.SetActive(false);
        }

        private void StopPlayback()
        {
            Speech.Stop();
        }
    }
}
