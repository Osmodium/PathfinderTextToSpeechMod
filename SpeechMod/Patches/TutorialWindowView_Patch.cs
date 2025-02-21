using HarmonyLib;
using Kingmaker.UI;
using Kingmaker.UI.MVVM._PCView.Tutorial;
using Kingmaker.UI.MVVM._VM.Tutorial;
using SpeechMod.Unity;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(TutorialWindowView<TutorialWindowVM>), "SetPage")]
public class TutorialWindowView_Patch
{
    public static void Postfix()
    {
        if (!Main.VoiceSettings.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(TutorialWindowView<TutorialWindowVM>)}_SetPage_Postfix");
#endif

        var smallWindow = UIHelper.TryFindInFadeCanvas("TutorialView/SmallWindow");
        var bigWindow = UIHelper.TryFindInFadeCanvas("TutorialView/BigWindow");

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

        var viewPort = smallWindow.TryFind("Window/Content/Body/ScrollView/ViewPort");
        if (viewPort == null)
        {
#if DEBUG
            Debug.LogWarning("ViewPort of SMALL tutorial window was not found!");
#endif
            return;
        }

        // Disable after first arrangement so when hovering buttons or links the view doesn't jump to top.
        var vlgw = viewPort.GetComponent<VerticalLayoutGroupWorkaround>();
        if (vlgw == null)
            return;

        vlgw.CalculateLayoutInputHorizontal();

        // Delay the disabling of the script until it has had a chance to run.
        var monoBehaviour = viewPort.GetComponent<MonoBehaviour>();
        monoBehaviour.ExecuteLater(0.5f, () => { vlgw.enabled = false; });
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