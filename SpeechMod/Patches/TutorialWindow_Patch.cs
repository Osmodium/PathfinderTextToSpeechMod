using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.Tutorial;
using Kingmaker.UI.MVVM._VM.Tutorial;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(TutorialWindowPCView<TutorialWindowVM>), "SetPage")]
public class TutorialWindow_Patch
{
    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(TutorialWindowPCView<TutorialWindowVM>)}_SetPage_Postfix");
#endif

        var smallWindow = Game.Instance.UI.FadeCanvas.transform.TryFind("TutorialView/SmallWindow");
        var bigWindow = Game.Instance.UI.FadeCanvas.transform.TryFind("TutorialView/BigWindow");

        if (smallWindow == null && bigWindow == null)
            Debug.LogWarning("Postfix on SetPage but both small and big tutorial window is null!");

        if (smallWindow != null && smallWindow.gameObject.activeInHierarchy)
            HookSmallWindow(smallWindow);

        if (bigWindow != null && bigWindow.gameObject.activeInHierarchy)
            HookBigWindow(bigWindow);
    }

    private static void HookSmallWindow(Transform smallWindow)
    {
#if DEBUG
            Debug.Log("Hooking on SMALL tutorial window!");
#endif

        var content = smallWindow.TryFind("Window/Content/Body/ScrollView/ViewPort/Content");
        if (content == null)
        {
#if DEBUG
            Debug.LogWarning("Content of SMALL tutorial window was not found!");
#endif
            return;
        }

        content.HookupTextToSpeechOnTransform();
    }

    private static void HookBigWindow(Transform bigWindow)
    {
#if DEBUG
        Debug.Log("Hooking on BIG tutorial window!");
#endif
        var rightSideTutorialDescription = bigWindow.TryFind("Window/Content/Body/RightSide/Description");
        if (rightSideTutorialDescription == null)
        {
#if DEBUG
            Debug.LogWarning("Right side description of BIG tutorial window was not found!");
#endif
            return;
        }

        rightSideTutorialDescription.HookupTextToSpeechOnTransform();
    }
}