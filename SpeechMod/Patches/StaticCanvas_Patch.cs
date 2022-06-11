using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.Tutorial;
using SpeechMod.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(StaticCanvas), "Initialize")]
public static class StaticCanvas_Patch
{
    private const string SCROLL_VIEW_PATH = "NestedCanvas1/DialogPCView/Body/View/Scroll View";

    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;
        Debug.Log($"{nameof(StaticCanvas)}_Initialize_Postfix @ {sceneName}");
#endif

        AddDialogSpeechButton();
        AddPlaybackControl();
    }

    private static void AddDialogSpeechButton()
    {

#if DEBUG
        Debug.Log("Adding speech button to dialog ui.");
#endif

        var parent = UIHelper.TryFindInStaticCanvas(SCROLL_VIEW_PATH);

        if (parent == null)
        {
            Debug.LogWarning("Parent not found!");
            return;
        }

        var buttonGameObject = ButtonFactory.CreatePlayButton(parent, () =>
        {
            Main.Speech.SpeakDialog(Game.Instance?.DialogController?.CurrentCue?.DisplayText);
        });

        buttonGameObject.name = "SpeechButton";
        buttonGameObject.transform.localPosition = new Vector3(-493, 164, 0);
        buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);

        buttonGameObject.SetActive(true);
    }

    private static void AddPlaybackControl()
    {
        var prefabPath = "TutorialView/BigWindow";
        var prefab = UIHelper.TryFindInFadeCanvas(prefabPath);

        if (prefab == null)
        {
            Debug.LogWarning($"Parent on path '{prefabPath}' not found!");
            return;
        }

        var playbackControl = Object.Instantiate(prefab, Game.Instance.UI.FadeCanvas.transform);
        playbackControl.name = "SpeechModPlaybackControl";

        Object.Destroy(playbackControl.GetComponent<TutorialModalWindowPCView>());

        var window = playbackControl.Find("Window");
        
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
            {
                Object.Destroy(child.gameObject);
            }
        }

        var contentFooter = window.TryFind("Content/Footer")?.gameObject;
        if (contentFooter)
            Object.Destroy(contentFooter);
        
        var rectControl = window.GetComponent<RectTransform>();
        rectControl.SetSize(new Vector2(400, 200));

        var background = window.TryFind("SheetMask")?.gameObject;
        if (background)
            background.gameObject.AddComponent<DragableWindow>();

        var closeButton = window.TryFind("OwlcatClose")?.gameObject;
        if (closeButton)
            closeButton.SetAction(() => { playbackControl.gameObject.SetActive(false); });

        var title = window.TryFind("Content/Header/Title")?.gameObject;
        if (title)
        {
            title.GetComponent<TextMeshProUGUI>().text = "Playback Control";
            foreach (Image image in title.GetComponentsInChildren<Image>())
            {
                image.raycastTarget = false;
            }
        }

        playbackControl.gameObject.SetActive(true);
    }
}