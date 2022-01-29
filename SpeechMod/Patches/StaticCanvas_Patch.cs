using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod;

[HarmonyPatch(typeof(StaticCanvas), "Initialize")]
public static class StaticCanvas_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var sceneName = Game.Instance.CurrentlyLoadedArea.ActiveUIScene.SceneName;

#if DEBUG
        Debug.Log($"{nameof(StaticCanvas)}_Initialize_Postfix @ {sceneName}");
#endif

        AddDialogSpeechButton();
    }

    private static void AddDialogSpeechButton()
    {

#if DEBUG
        Debug.Log("Adding speech button to dialog ui.");
#endif

        var parent = UIHelper.TryFindInStaticCanvas("DialogPCView/Body/View/Scroll View");

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