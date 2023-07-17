using System;
using DG.Tweening;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.Tutorial;
using Kingmaker.Utility;
using Owlcat.Runtime.UI.Controls.Button;
using SpeechMod.Unity;
using SpeechMod.Unity.Utility;
using TMPro;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Kingmaker.Assets.UI.Common;

public class PlaybackControl : MonoBehaviour
{
    private static Transform m_PlaybackControl;
    private static TextMeshProUGUI m_Title;
    private static TextMeshProUGUI m_ProgressText;
    private static Slider m_ProgressSlider;
    private static SpriteState m_StopButtonSpriteState;

    public static void TryInstantiate()
    {
        if (m_PlaybackControl)
            return;

#if DEBUG
        Debug.Log("Initializing playback control...");
#endif

        var prefabPath = "TutorialView/BigWindow";
        var prefab = UIHelper.TryFindInFadeCanvas(prefabPath);

        if (prefab == null)
        {
            Debug.LogWarning($"Parent on path '{prefabPath}' not found!");
            return;
        }

        m_PlaybackControl = Instantiate(prefab, Game.Instance.UI.FadeCanvas.transform);
        m_PlaybackControl.transform.transform.ResetScaleAndPosition();
        m_PlaybackControl.name = "SpeechModPlaybackControl";
        m_PlaybackControl.gameObject.AddComponent<PlaybackControl>();

        Destroy(m_PlaybackControl.GetComponent<TutorialModalWindowPCView>());

        var window = m_PlaybackControl.Find("Window");

        var attentionmarker = window.TryFind("AttentionMarker")?.gameObject;
        if (attentionmarker)
            Destroy(attentionmarker);

        var art = window.TryFind("SheetMask/Sheet/Art")?.gameObject;
        if (art)
            Destroy(art);

        var contentBody = window.TryFind("Content/Body")?.gameObject;
        if (contentBody)
        {
            foreach (Transform child in contentBody.transform)
                Destroy(child.gameObject);
            contentBody.transform.ResetScaleAndPosition();
        }

        var contentFooter = window.TryFind("Content/Footer")?.gameObject;
        if (contentFooter)
        {
            var spriteTransform = contentFooter.transform.TryFind("Toggle");
            var spriteButton = spriteTransform.GetComponent<ToggleWorkaround>();
            var st = spriteButton.spriteState;
            m_StopButtonSpriteState = new SpriteState
            {
                disabledSprite = st.disabledSprite,
                highlightedSprite = st.highlightedSprite,
                pressedSprite = st.pressedSprite,
                selectedSprite = st.selectedSprite
            };
            Destroy(contentFooter);
        }

        var rectControl = window.GetComponent<RectTransform>();
        rectControl.SetSize(new Vector2(380, 120));

        var background = window.TryFind("SheetMask")?.gameObject;
        if (background)
            background.gameObject.AddComponent<DraggableWindow>();

        var closeButton = window.TryFind("OwlcatClose")?.gameObject;
        if (closeButton)
            closeButton.SetAction(() => { m_PlaybackControl.gameObject.SetActive(false); });

        closeButton.RectAlignTopRight();
        var closeButtonRectTransform = closeButton.GetComponent<RectTransform>();
        closeButtonRectTransform.anchoredPosition = new Vector2(-10, -10);

        var title = window.TryFind("Content/Header/Title")?.gameObject;
        if (title)
        {
            m_Title = title.GetComponent<TextMeshProUGUI>();
            m_Title.text = "Playback Control";
            foreach (Image image in m_Title.gameObject.GetComponentsInChildren<Image>())
                image.raycastTarget = false;
        }

        var content = window.TryFind("Content")?.gameObject;
        var contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.anchoredPosition = new Vector2(contentRectTransform.anchoredPosition.x, 20f);

        //var titleRectTransform = m_Title.GetComponent<RectTransform>();
        //titleRectTransform.anchoredPosition = new Vector2(titleRectTransform.anchoredPosition.x, 20f);

        // Add content to body
        AddBody(contentBody);

        m_PlaybackControl.gameObject.SetActive(true);
    }

    private static void AddBody(GameObject body)
    {
        Debug.Log("Adding body...");

        DestroyImmediate(body.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>());
        body.gameObject.AddComponent<VerticalLayoutGroup>();

        var bodyContainer = new GameObject("Container");
        bodyContainer.transform.SetParent(body.transform);
        bodyContainer.transform.transform.ResetScaleAndPosition();
        //bodyContainer.transform.localPosition = body.transform.localPosition;
        var bodyContainerRectTransform = bodyContainer.AddComponent<RectTransform>();
        //bodyContainer.AddComponent<VerticalLayoutGroupWorkaround>();

        // Label

        var labelContainer = new GameObject("StatusLabel");
        labelContainer.transform.SetParent(bodyContainer.transform);
        labelContainer.transform.transform.ResetScaleAndPosition();
        var labelContainerRectTransform = labelContainer.AddComponent<RectTransform>();
        labelContainerRectTransform.SetHeight(20);

        var labelText = labelContainer.AddComponent<TextMeshProUGUI>();
        labelText.autoSizeTextContainer = false;
        labelText.enableAutoSizing = true;
        labelText.SetText("Idle... Waiting for voice to play...");
        labelText.fontSizeMin = 20;
        labelText.fontSizeMax = 20;
        labelText.font = m_Title.font;
        labelText.color = m_Title.color;

        // Progressbar
        var sliderContainer = new GameObject("ProgressBar");
        sliderContainer.transform.SetParent(bodyContainer.transform);
        sliderContainer.transform.transform.ResetScaleAndPosition();
        var sliderContainerRectTransform = sliderContainer.AddComponent<RectTransform>();
        sliderContainerRectTransform.SetHeight(16);

        var sliderBackground = new GameObject("Background");
        sliderBackground.transform.SetParent(sliderContainer.transform);
        sliderBackground.transform.transform.ResetScaleAndPosition();
        var sliderBackgroundRectTransform = sliderBackground.AddComponent<RectTransform>();
        sliderBackgroundRectTransform.FillParent();
        var sliderBackgroundImage = sliderBackground.AddComponent<Image>();
        sliderBackgroundImage.raycastTarget = false;
        sliderBackgroundImage.fillMethod = Image.FillMethod.Horizontal;
        sliderBackgroundImage.color = Color.black;
        sliderBackgroundImage.fillOrigin = 0;
        sliderBackgroundRectTransform.SetSize(new Vector2(200, 16));
        sliderBackgroundImage.sprite = AssetLoader.LoadInternal("Sprites", "UI_BackgroundExpiriensPB.png", new Vector2Int(200, 20));

        var sliderFillArea = new GameObject("Fill Area");
        sliderFillArea.transform.SetParent(sliderContainer.transform);
        sliderFillArea.transform.transform.ResetScaleAndPosition();

        var sliderFillAreaRectTransform = sliderFillArea.AddComponent<RectTransform>();
        sliderFillAreaRectTransform.FillParent();

        var sliderFill = new GameObject("Fill");
        sliderFill.transform.SetParent(sliderFillArea.transform);
        sliderFill.transform.transform.ResetScaleAndPosition();
        var sliderFillRectTransform = sliderFill.AddComponent<RectTransform>();
        sliderFillRectTransform.FillParent();

        var sliderFillBackground = new GameObject("Background");
        sliderFillBackground.transform.SetParent(sliderFill.transform);
        sliderFillBackground.transform.transform.ResetScaleAndPosition();
        var sliderFillBackgroundRectTransform = sliderFillBackground.AddComponent<RectTransform>();
        sliderFillBackgroundRectTransform.FillParent();
        var sliderFillBackgroundImage = sliderFillBackground.AddComponent<Image>();
        sliderFillBackgroundImage.raycastTarget = false;
        sliderFillBackgroundImage.fillMethod = Image.FillMethod.Horizontal;
        sliderFillBackgroundImage.fillOrigin = 0;
        sliderFillBackgroundImage.color = Color.gray;
        sliderFillBackgroundImage.DOFillAmount(0.4f, 2f);
        sliderFillBackgroundRectTransform.SetSize(new Vector2(180, 18));
        sliderFillBackgroundImage.sprite = AssetLoader.LoadInternal("Sprites", "UI_ExpiriensPB.png", new Vector2Int(200, 20));

        var sliderHandleSlideArea = new GameObject("Handle Slide Area");
        sliderHandleSlideArea.transform.SetParent(sliderContainer.transform);
        sliderHandleSlideArea.transform.transform.ResetScaleAndPosition();
        var sliderHandleSlideAreaRectTransform = sliderHandleSlideArea.AddComponent<RectTransform>();
        sliderHandleSlideAreaRectTransform.FillParent();

        var sliderHandle = new GameObject("Handle");
        sliderHandle.transform.SetParent(sliderHandleSlideArea.transform);
        sliderHandle.transform.ResetScaleAndPosition();
        var sliderHandleRectTransform = sliderHandle.AddComponent<RectTransform>();
        sliderHandleRectTransform.FillParent();
        var sliderHandleImage = sliderHandle.AddComponent<Image>();
        sliderHandleImage.raycastTarget = false;
        sliderHandleRectTransform.SetSize(new Vector2(8, 18));
        sliderHandleImage.sprite = AssetLoader.LoadInternal("Sprites", "UI_ExpiriensPBLine.png", new Vector2Int(11, 43));

        // Stop button
        var stopButton = ButtonFactory.CreateOwlcatButton(bodyContainer.transform, m_StopButtonSpriteState, "Stop playback", null);
        //stopButton.transform.localScale = Vector3.one;
        stopButton.transform.ResetScaleAndPosition();
        stopButton.transform.position = new Vector3(300, 100, sliderHandle.transform.localPosition.z);

        //labelContainer.transform.localPosition = bodyContainer.transform.localPosition;
        //sliderContainer.transform.localPosition = bodyContainer.transform.localPosition;


        // // Status & Progress
        // //ServiceWindowsPCView/CharacterInfoPCView/CharacterScreen/LevelClassScores/LevelBox/Experience/
        // //ServiceWindowsPCView/InventoryPCView/Inventory/LevelClassScores/LevelBox/Experience/
        // var progressBarContainerPrefab = UIHelper.TryFindInStaticCanvas("ServiceWindowsPCView/CharacterInfoPCView/CharacterScreen/LevelClassScores/LevelBox/Experience/Container");
        // if (!progressBarContainerPrefab)
        //     return;
        //
        // var progressBar = Instantiate(progressBarContainerPrefab, body.transform);
        //
        // progressBar.ResetAll();
        // var progressBarRectTransform = progressBar.GetComponent<RectTransform>();
        // progressBarRectTransform.SetLeftTopPosition(new Vector2(progressBarRectTransform.localPosition.x, 50f));
        //
        // Destroy(progressBar.TryFind("NegativeLevelField").gameObject);
        // Destroy(progressBar.TryFind("Experience counter").gameObject);
        //
        // progressBarRectTransform.FillParent();
        //
        // progressBarRectTransform.sizeDelta = new Vector2(350, progressBarRectTransform.sizeDelta.y);
        //
        // var progressTextTransform = progressBar.TryFind("Experience Label");
        // var progressTextRectTransform = progressTextTransform.GetComponent<RectTransform>();
        // progressTextRectTransform.SetWidth(progressBarRectTransform.GetWidth() - 40f);
        // progressTextRectTransform.anchoredPosition = Vector2.zero;
        //
        // m_ProgressText = progressTextTransform.GetComponent<TextMeshProUGUI>();
        // m_ProgressText.text = "Idle";
        //
        // var progressSliderTransform = progressBar.TryFind("ExperienceSlider");
        // //var progressSliderRectTransform = progressSliderTransform.GetComponent<RectTransform>();
        //
        // m_ProgressSlider = progressSliderTransform.GetComponent<Slider>();
        // m_ProgressSlider.value = 0;

        // Stopbutton
    }

    // private void LateUpdate()
    // {
    //
    // }

    private void OnDestroy()
    {
        m_PlaybackControl = null;
    }
}