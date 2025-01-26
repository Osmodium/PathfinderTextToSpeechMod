using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using UnityEngine;

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
}