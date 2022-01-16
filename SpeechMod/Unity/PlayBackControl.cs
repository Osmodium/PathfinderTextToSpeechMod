using System;
using System.Collections;
using System.Security.Cryptography;
using Kingmaker;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.Tooltip.Templates;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using SpeechMod.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace SpeechMod.Unity;

public class PlaybackControl : MonoBehaviour
{
    //public static PlaybackControl Instance;
    private static Color m_PlaybackProgressColor = Color.green;

    public static void UpdatePlaybackProgessColor()
    {
        m_PlaybackProgressColor = new Color(Main.Settings.PlaybackColorR, Main.Settings.PlaybackColorG, Main.Settings.PlaybackColorB, Main.Settings.PlaybackColorA);
    }

    public static void TryInstantiate()
    {
        //if (Instance != null)
        //    return;
#if DEBUG
        Debug.Log($"{nameof(PlaybackControl)}_TryInstantiate @ {Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName}");
#endif
        var canvasTransform = UIUtility.IsGlobalMap() ? Game.Instance.UI?.GlobalMapUI?.transform : Game.Instance.UI?.Canvas?.transform;

        if (canvasTransform == null)
        {
            Debug.LogWarning("Could not find canvas for scene!");
            return;
        }

        Debug.Log(canvasTransform.name + "/" + canvasTransform.parent.name);

        try
        {

            var canvasGameObject = new GameObject("SpeechModStaticUI")
            {
                layer = canvasTransform.gameObject.layer
            };
            canvasGameObject.transform.SetParent(canvasTransform.transform);
            canvasGameObject.transform.localPosition = Vector3.zero;
            canvasGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            canvasGameObject.transform.localScale = Vector3.one;
            var playbackControl = canvasGameObject.gameObject.AddComponent<PlaybackControl>();
            playbackControl.Initialize();

        }
        catch
        {
            Debug.Log($"{nameof(PlaybackControl)}_TryInstantiate: Failed!");
        }
    }

    private bool m_HasSpoken;
    private GameObject m_ProgressBarGameObject;
    private RectTransform m_ImageRectTransform;
    private Image m_Progressbar;
    private GameObject m_TextGameObject;
    private TextMeshProUGUI m_TextMeshProUGUI;
    private GameObject m_StopButton;

    public void Initialize()
    {
        try
        {
            var uiGameObject = UIUtility.IsGlobalMap() ? Game.Instance?.UI?.GlobalMapUI?.gameObject : Game.Instance?.UI?.Canvas?.gameObject;

            if (uiGameObject == null)
                return;

            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.transform.localPosition = Vector3.zero;
            rectTransform.transform.localRotation = Quaternion.Euler(Vector3.zero);
            rectTransform.transform.localScale = Vector3.one;

            rectTransform.SetSize(Vector2.zero);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = Vector2.zero;

            m_ProgressBarGameObject = new GameObject("ProgressImage")
            {
                layer = uiGameObject.layer
            };
            m_ProgressBarGameObject.SetActive(false);
            m_ProgressBarGameObject.transform.SetParent(transform);
            m_ProgressBarGameObject.transform.localPosition = Vector3.zero;
            m_ProgressBarGameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            m_ProgressBarGameObject.transform.localScale = new Vector3(0, 1, 1);

            m_ImageRectTransform = m_ProgressBarGameObject.AddComponent<RectTransform>();
            m_ImageRectTransform.SetSize(Vector2.zero);
            m_ImageRectTransform.anchorMin = new Vector2(0, 1);
            m_ImageRectTransform.anchorMax = Vector2.one;
            m_ImageRectTransform.pivot = new Vector2(0, 1);
            m_ImageRectTransform.anchoredPosition = Vector2.zero;
            m_ImageRectTransform.SetHeight(5);
            m_Progressbar = m_ProgressBarGameObject.AddComponent<Image>();

            //m_StopButton = ButtonFactory.CreateSquareButton();
            m_StopButton = ButtonFactory.CreatePlayButton(transform, StopPlayback);
            if (m_StopButton != null)
            {
                m_StopButton.SetActive(false);
                var buttonRectTransform = m_StopButton.GetComponent<RectTransform>();
                buttonRectTransform.anchorMin = new Vector2(0, 1);
                buttonRectTransform.anchorMax = new Vector2(0, 1);
                buttonRectTransform.pivot = new Vector2(0, 1);
                m_StopButton.transform.SetParent(transform);
                m_StopButton.transform.localPosition = Vector3.zero;
                m_StopButton.transform.localRotation = Quaternion.Euler(Vector3.zero);
                m_StopButton.transform.localScale = Vector3.one;
                //DestroyImmediate(m_StopButton.transform.GetChild(0));

                var staticRoot = uiGameObject.transform;
                var failedImagePrefab = staticRoot.TryFind("ServiceWindowsPCView/JournalPCView/JournalQuestView/BodyGroup/ObjectivesGroup/StandardScrollView/Viewport/Content/JournalQuestObjectiveView/MultiButton/FailedImage");
                if (failedImagePrefab == null)
                {
                    Debug.LogWarning("Can't find failed image prefab!");
                }
                else
                {
                    var failedImageTransform = Instantiate(failedImagePrefab);
                    failedImageTransform.SetParent(m_StopButton.transform);
                    failedImageTransform.localPosition = Vector3.zero;
                    failedImageTransform.localRotation = Quaternion.Euler(Vector3.zero);
                    failedImageTransform.localScale = Vector3.zero;
                }
            }
            //OwlcatButton owlCatButton = m_StopButton.GetComponentInChildren<OwlcatButton>();
            //owlCatButton.OnLeftClick.AddListener(StopPlayback);
            //owlCatButton.SetTooltip(new TooltipTemplateSimple("Stop TTS Playback", "Stops the current text to speech playback."), new TooltipConfig {
            //    InfoCallMethod = InfoCallMethod.None
            //});

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
        catch
        {
            Debug.LogWarning($"{nameof(PlaybackControl)}_Initialize: Could not initalize playbackcontrol.");
            Destroy(gameObject);
        }
    }

    public void Awake()
    {
        m_HasSpoken = false;
        Debug.Log($"{nameof(PlaybackControl)} Awake");
        //if (Instance != null)
        //    Destroy(gameObject);
        //else
        //    Instance = this;
    }

    public void OnDestroy()
    {
        //if (Instance == this)
        //    Instance = null;
        Debug.Log($"{nameof(PlaybackControl)} OnDestroy");
    }

    public void Update()
    {
        //if (ShouldSkip())
        //    return;

        if (WindowsVoiceUnity.IsSpeaking)
        {
            m_HasSpoken = true;
            if (!Main.Settings.ShowPlaybackProgress)
                return;
#if DEBUG
            if (m_TextGameObject != null)
            {
                if (!m_TextGameObject.activeSelf)
                    m_TextGameObject.SetActive(true);
                m_TextMeshProUGUI.text = $"{WindowsVoiceUnity.WordPosition}/{WindowsVoiceUnity.WordCount} : {WindowsVoiceUnity.GetNormalizedProgress()}";
            }
#endif

            if (m_ProgressBarGameObject != null)
            {
                if (!m_ProgressBarGameObject.activeSelf)
                    m_ProgressBarGameObject.SetActive(true);
                m_Progressbar.color = m_PlaybackProgressColor;
                m_ProgressBarGameObject.transform.localScale = Vector3.Slerp(m_ProgressBarGameObject.transform.localScale, new Vector3(WindowsVoiceUnity.GetNormalizedProgress(), 1, 1), 0.05f);
            }

            if (m_StopButton != null)
            {
                if (!m_StopButton.activeSelf)
                    m_StopButton.SetActive(true);
            }
        }
        else
        {
#if DEBUG
            if (m_TextGameObject != null)
            {
                if (m_TextGameObject.activeSelf)
                    m_TextGameObject.SetActive(false);
            }
#endif
            if (m_ProgressBarGameObject != null)
            {
                //m_ProgressBarGameObject.transform.localScale = Vector3.Slerp(m_ProgressBarGameObject.transform.localScale, new Vector3(1, 1, 1), 0.05f);
                if (m_ProgressBarGameObject.activeSelf)
                {
                    m_ProgressBarGameObject.SetActive(false);
                    m_ProgressBarGameObject.transform.localScale = new Vector3(0, 1, 1);
                }
            }

            if (m_StopButton != null)
            {
                if (m_StopButton.activeSelf)
                    m_StopButton.SetActive(false);
            }

            if (m_HasSpoken)
                Destroy(gameObject);
        }
    }

    private bool ShouldSkip()
    {
        bool shouldSkip = false;
#if DEBUG
        if (m_TextGameObject == null)
        {
            Debug.LogWarning($"[{nameof(PlaybackControl)}] m_TextGameObject is null!");
            shouldSkip = true;
        }
#endif

        if (m_ProgressBarGameObject == null)
        {
            Debug.LogWarning($"[{nameof(PlaybackControl)}] m_ProgressBarGameObject is null!");
            shouldSkip = true;
        }

        if (m_StopButton == null)
        {
            Debug.LogWarning($"[{nameof(PlaybackControl)}] m_StopButton is null!");
            shouldSkip = true;
        }

        return shouldSkip;
    }

    private void StopPlayback()
    {
        Speech.Stop();
    }
}