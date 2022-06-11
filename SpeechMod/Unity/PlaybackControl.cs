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
    private static GameObject m_Title;
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

        m_PlaybackControl = Object.Instantiate(prefab, Game.Instance.UI.FadeCanvas.transform);
        m_PlaybackControl.name = "SpeechModPlaybackControl";
        m_PlaybackControl.gameObject.AddComponent<PlaybackControl>();

        Object.Destroy(m_PlaybackControl.GetComponent<TutorialModalWindowPCView>());

        var window = m_PlaybackControl.Find("Window");

        var attentionmarker = window.TryFind("AttentionMarker")?.gameObject;
        if (attentionmarker)
            Object.Destroy(attentionmarker);

        var art = window.TryFind("SheetMask/Sheet/Art")?.gameObject;
        if (art)
            Object.Destroy(art);

        var contentBody = window.TryFind("Content/Body")?.gameObject;
        if (contentBody)
        {
            foreach (Transform child in contentBody.transform)
                Object.Destroy(child.gameObject);
        }

        var contentFooter = window.TryFind("Content/Footer")?.gameObject;
        if (contentFooter)
            Object.Destroy(contentFooter);
        
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

        m_Title = window.TryFind("Content/Header/Title")?.gameObject;
        if (m_Title)
        {
            m_Title.GetComponent<TextMeshProUGUI>().text = "Playback Control";
            foreach (Image image in m_Title.GetComponentsInChildren<Image>())
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

        bodyContainer.AddComponent<RectTransform>();
        
        

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