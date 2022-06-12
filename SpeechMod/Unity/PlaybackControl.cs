using DG.Tweening;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.Tutorial;
using Kingmaker.Utility;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackControl : MonoBehaviour
{
    private static Transform m_PlaybackControl;
    private static TextMeshProUGUI m_Title;
    private static TextMeshProUGUI m_ProgressText;
    private static Slider m_ProgressSlider;

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
        }

        var contentFooter = window.TryFind("Content/Footer")?.gameObject;
        if (contentFooter)
            Destroy(contentFooter);

        var rectControl = window.GetComponent<RectTransform>();
        rectControl.SetSize(new Vector2(400, 180));

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

        var bodyContainer = new GameObject("Conainer");
        bodyContainer.transform.SetParent(body.transform);
        bodyContainer.transform.localPosition = body.transform.localPosition;
        var bodyContainerRectTransform = bodyContainer.AddComponent<RectTransform>();
        //bodyContainer.AddComponent<VerticalLayoutGroupWorkaround>();

        // Label

        var labelContainer = new GameObject("StatusLabel");
        labelContainer.transform.SetParent(bodyContainer.transform);
        var labelContainerRectTransform = labelContainer.AddComponent<RectTransform>();
        labelContainerRectTransform.SetHeight(20);

        var labelText = labelContainer.AddComponent<TextMeshProUGUI>();
        labelText.autoSizeTextContainer = false;
        labelText.enableAutoSizing = true;
        labelText.SetText("Idle... Waiting for voice to play...");
        //labelText.fontSizeMin = 20;
        //labelText.fontSizeMax = 20;
        labelText.font = m_Title.font;
        labelText.color = m_Title.color;

        // Progressbar

        var sliderContainer = new GameObject("ProgressBar");
        sliderContainer.transform.SetParent(bodyContainer.transform);
        var sliderContainerRectTransform = sliderContainer.AddComponent<RectTransform>();
        sliderContainerRectTransform.SetHeight(16);

        var sliderBackground = new GameObject("Background");
        sliderBackground.transform.SetParent(sliderContainer.transform);
        var sliderBackgroundRectTransform = sliderBackground.AddComponent<RectTransform>();
        sliderBackgroundRectTransform.FillParent();
        var sliderBackgroundImage = sliderBackground.AddComponent<Image>();
        sliderBackgroundImage.raycastTarget = false;
        sliderBackgroundImage.fillMethod = Image.FillMethod.Horizontal;
        sliderBackgroundImage.color = Color.black;
        sliderBackgroundImage.fillOrigin = 0;
        //sliderBackgroundRectTransform.SetSize(new Vector2(200, 20));
        // TODO Set texture
        //sliderBackgroundImage.overrideSprite = Sprite.Create(Texture2D.redTexture, new Rect(0, 0, 200, 20), new Vector2(0f, 0.5f), 100);

        var sliderFillArea = new GameObject("Fill Area");
        sliderFillArea.transform.SetParent(sliderContainer.transform);

        var sliderFillAreaRectTransform = sliderFillArea.AddComponent<RectTransform>();
        sliderFillAreaRectTransform.FillParent();

        var sliderFill = new GameObject("Fill");
        sliderFill.transform.SetParent(sliderFillArea.transform);
        var sliderFillRectTransform = sliderFill.AddComponent<RectTransform>();
        sliderFillRectTransform.FillParent();

        var sliderFillBackground = new GameObject("Background");
        sliderFillBackground.transform.SetParent(sliderFill.transform);
        var sliderFillBackgroundRectTransform = sliderFillBackground.AddComponent<RectTransform>();
        sliderFillBackgroundRectTransform.FillParent();
        var sliderFillBackgroundImage = sliderFillBackground.AddComponent<Image>();
        sliderFillBackgroundImage.raycastTarget = false;
        sliderFillBackgroundImage.fillMethod = Image.FillMethod.Horizontal;
        sliderFillBackgroundImage.fillOrigin = 0;
        sliderFillBackgroundImage.color = Color.gray;
        sliderFillBackgroundImage.DOFillAmount(0.4f, 2f);
        //sliderBackgroundRectTransform.SetSize(new Vector2(200, 20));
        // TODO Set texture
        //sliderFillBackgroundImage.overrideSprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 120, 20), new Vector2(0f, 0.5f), 100);

        var sliderHandleSlideArea = new GameObject("Handle Slide Area");
        sliderHandleSlideArea.transform.SetParent(sliderContainer.transform);
        var sliderHandleSlideAreaRectTransform = sliderHandleSlideArea.AddComponent<RectTransform>();
        sliderHandleSlideAreaRectTransform.FillParent();

        var sliderHandle = new GameObject("Handle");
        sliderHandle.transform.SetParent(sliderHandleSlideArea.transform);
        var sliderHandleRectTransform = sliderHandle.AddComponent<RectTransform>();
        sliderHandleRectTransform.FillParent();


        // Button
        var stopButtonGameObject = new GameObject("StopButton");
        stopButtonGameObject.transform.SetParent(bodyContainer.transform);
        stopButtonGameObject.AddComponent<RectTransform>();
        var buttonImage = stopButtonGameObject.AddComponent<Image>();
        //buttonImage.sprite = Resources.Load<Sprite>("UI_BoxButton_Default");
        
        //var sl = new SpriteLink
        //{
        //    AssetId = "237"
        //};
        //buttonImage.sprite = sl.Load();
        //var stopButton = stopButtonGameObject.AddComponent<OwlcatButton>();
        //AssetsLoader
        //stopButton

        labelContainer.transform.localPosition = bodyContainer.transform.localPosition;
        sliderContainer.transform.localPosition = bodyContainer.transform.localPosition;


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