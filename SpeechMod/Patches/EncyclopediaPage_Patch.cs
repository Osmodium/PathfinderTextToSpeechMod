using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Encyclopedia;
using SpeechMod.Unity;
using SpeechMod.Voice;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches
{

    [HarmonyPatch(typeof(EncyclopediaPagePCView), "UpdateView")]
    public static class EncyclopediaPage_Patch
    {
        private static readonly string m_ButtonName = "EncyclopediaSpeechButton";

        public static void Postfix()
        {

            var bodyGroup = Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView/EncyclopediaPCView/EncyclopediaPageView/BodyGroup");
            if (bodyGroup == null)
            {
                Debug.Log("Couldn't find BodyGroup...");
                return;
            }

            var content = bodyGroup.Find("ObjectivesGroup/StandardScrollView/Viewport/Content");
            if (content == null)
            {
                Debug.Log("Couldn't any TextMeshProUGUI...");
                return;
            }

            var allTexts = content.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (allTexts == null || allTexts.Length == 0)
            {
                Debug.Log("Couldn't any TextMeshProUGUI...");
                return;
            }

            foreach (var textMeshPro in allTexts)
            {
                var parent = textMeshPro.transform;

                GameObject button = null;
                try
                {
                    button = parent.Find(m_ButtonName).gameObject;
                }
                catch
                { } // Sigh...

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
                    Speech.Speak(textMeshPro.text);
                });
                button.name = m_ButtonName;
                button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                button.RectAlignTopLeft();
                button.transform.localPosition = new Vector3(-36, -26, 0);
                button.gameObject.SetActive(true);
            }
        }
    }
}
