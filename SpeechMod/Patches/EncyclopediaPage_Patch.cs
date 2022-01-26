using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Encyclopedia;
using SpeechMod.Unity;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch(typeof(EncyclopediaPagePCView), "UpdateView")]
public static class EncyclopediaPage_Patch
{
    private static readonly string m_ButtonName = "EncyclopediaSpeechButton";

    public static void Postfix()
    {
        if (!Main.Enabled)
            return;

        var bodyGroup = Game.Instance.UI.Canvas.transform.TryFind("ServiceWindowsPCView/EncyclopediaPCView/EncyclopediaPageView/BodyGroup");
        if (bodyGroup == null)
        {
#if DEBUG
            Debug.Log("Couldn't find BodyGroup...");
#endif
            return;
        }

        var content = bodyGroup.TryFind("ObjectivesGroup/StandardScrollView/Viewport/Content");
        if (content == null)
        {
#if DEBUG
            Debug.Log("Couldn't any TextMeshProUGUI...");
#endif
            return;
        }

        // Only get the texts that is not in the unit view.
        var allTexts = content.gameObject?.GetComponentsInChildren<TextMeshProUGUI>(true).Where(t => t.transform.name.Equals("Text")).ToArray();
        if (allTexts == null || allTexts.Length == 0)
        {
#if DEBUG
            Debug.Log("Couldn't find any TextMeshProUGUI...");
#endif
            return;
        }

        foreach (var textMeshPro in allTexts)
        {
            var parent = textMeshPro.transform;

            var button = parent.TryFind(m_ButtonName)?.gameObject;

            if (button != null)
            {
#if DEBUG
                Debug.Log("Button already added, relocating and activating...");
#endif
                button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                button.RectAlignTopLeft();
                button.transform.localPosition = new Vector3(-36, -26, 0);
                continue;
            }

#if DEBUG
            Debug.Log("Adding playbutton...");
#endif
            button = ButtonFactory.CreatePlayButton(parent, () =>
            {
                Main.Speech.Speak(textMeshPro.text);
            });
            button.name = m_ButtonName;
            button.transform.localRotation = Quaternion.Euler(0, 0, 90);
            button.RectAlignTopLeft();
            button.transform.localPosition = new Vector3(-36, -26, 0);
            button.gameObject.SetActive(true);
        }
    }
}