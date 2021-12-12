using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Journal;
using SpeechMod.Unity;
using SpeechMod.Voice;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod.Patches
{
    [HarmonyPatch(typeof(JournalQuestObjectivePCView), "BindViewImplementation")]
    public static class JournalQuestObjective_Patch
    {
        private static readonly string m_ButtonName = "JQSpeechButton";

        static void Postfix()
        {
#if DEBUG
            Debug.Log("JournalQuestObjectivePCView_BindViewImplementation");
#endif

            var bodyGroup = Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView/JournalPCView/JournalQuestView/BodyGroup");
            if (bodyGroup == null)
            {
                Debug.Log("Couldn't find BodyGroup...");
                return;
            }

            var allTexts = bodyGroup.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (allTexts == null || allTexts.Length == 0)
            {
                Debug.Log("Couldn't any TextMeshProUGUI...");
                return;
            }

            bool isFirst = true;
            foreach (var textMeshPro in allTexts)
            {
                var tmpTransform = textMeshPro.transform;
                if (!ShouldAddButton(tmpTransform))
                    continue;
                
                GameObject button = null;
                try
                {
                    button = tmpTransform.Find(m_ButtonName).gameObject;
                }
                catch
                { } // Sigh...

                if (button != null)
                {
#if DEBUG
                    Debug.Log("Button already added, relocating and activating...");
#endif
                    button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    tmpTransform.gameObject.RectAlignTopLeft();
                    button.RectAlignTopLeft();
                    button.transform.localPosition = GetNewPosition(tmpTransform, ref isFirst);
                    continue;
                }

#if DEBUG
                Debug.Log("Adding playbutton...");
#endif
                button = ButtonFactory.CreatePlayButton(tmpTransform.transform, () =>
                {
                    Speech.Speak(textMeshPro.text);
                });
                button.name = m_ButtonName;
                button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                tmpTransform.gameObject.RectAlignTopLeft();
                button.RectAlignTopLeft();
                button.transform.localPosition = GetNewPosition(tmpTransform, ref isFirst);
                button.SetActive(true);
            }

            // Move the line back behind our buttons.
            var allImages = bodyGroup.GetComponentsInChildren<Image>();
            foreach (var image in allImages)
            {
                if (image.gameObject.name.Equals("LeftVerticalBorderImage"))
                    image.transform.SetAsFirstSibling();
            }
        }

        private static bool ShouldAddButton(Transform transform)
        {
            switch (transform.name)
            {
                case "DescriptionLabel":
                case "Label":
                    return true;
                default:
                    return false;
            }
        }

        private static Vector3 GetNewPosition(Transform transform, ref bool isFirst)
        {
            switch (transform.name)
            {
                case "LastChapterLabel":
                    return new Vector3(-72, -35, 0);
                case "TitleLabel":
                    return new Vector3(0, -42, 0);
                case "DescriptionLabel":
                {
                    if (!isFirst)
                        return new Vector3(-35, -24, 0);
                    isFirst = false;
                    return new Vector3(-10, -24, 0);
                }
                case "Label":
                    return new Vector3(-82, -26, 0);
                default:
                    return Vector3.zero;
            }
        }
    }
}
