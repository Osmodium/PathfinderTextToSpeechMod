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

        public static void Postfix()
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
                    button = tmpTransform?.Find(m_ButtonName)?.gameObject;
                }
                catch {} // Sigh...

                if (button != null)
                {
#if DEBUG
                    Debug.Log("Button already added, relocating and activating...");
#endif
                    button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    tmpTransform.gameObject.RectAlignTopLeft();
                    button.RectAlignTopLeft();
                    SetNewPosition(tmpTransform, button.transform, ref isFirst);
                    button.SetActive(true);
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
                SetNewPosition(tmpTransform, button.transform, ref isFirst);
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
                case "LastChapterLabel":
                case "DescriptionLabel":
                case "Label":
                    return true;
                default:
                    return false;
            }
        }

        private static void SetNewPosition(Transform tmpTransform, Transform transform, ref bool isFirst)
        {
            switch (tmpTransform.name)
            {
                case "LastChapterLabel":
                    transform.localPosition = new Vector3(-72, -35, 0);
                    break;
                case "TitleLabel":
                    transform.localPosition = new Vector3(0, -42, 0);
                    break;
                case "DescriptionLabel":
                    if (isFirst)
                    {
                        isFirst = false;
                        transform.localPosition = new Vector3(-10, -24, 0);
                        break;
                    }
                    transform.localPosition = new Vector3(-35, -24, 0);
                    break;
                case "Label":
                    GameObject ipi = null;
                    try
                    {
                        ipi = tmpTransform.parent.Find("InProgressImage").gameObject;
                    }
                    catch { }//sigh
                    transform.localPosition = new Vector3(-82, ipi.transform.InverseTransformPoint(ipi.transform.position).y - 26, 0);
                    break;
                default:
                    transform.localPosition = Vector3.zero;
                    break;
            }
        }
    }
}
