using HarmonyLib;
using Kingmaker;
using SpeechMod.Unity;
using SpeechMod.Voice;
using System;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Journal;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using TMPro;
using UniRx;
using UnityEngine;

namespace SpeechMod.Patches
{
    [HarmonyPatch(typeof(JournalQuestObjectivePCView), "BindViewImplementation")]
    public static class JournalQuestObjective_Patch
    {
        private static readonly string m_ButtonName = "JournalQuestSpeechButton";

        static void Postfix()
        {
            Debug.Log("JournalQuestObjectivePCView_BindViewImplementation");

            var bodyGroup = Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView/JournalPCView/JournalQuestView/BodyGroup");
            if (bodyGroup == null)
                return;

            var allTexts = bodyGroup.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (allTexts == null || allTexts.Length == 0)
                return;

            foreach (var textMeshPro in allTexts)
            {
                var parent = textMeshPro.transform;

                var button = parent.Find(m_ButtonName).gameObject;
                if (button != null)
                {
                    button.transform.localPosition = Vector3.zero;
                    button.SetActive(true);
                    continue;
                }

                Debug.Log("Adding playbutton...");
                button = ButtonFactory.CreatePlayButton(parent, () =>
                {
                    Speech.Speak(textMeshPro.text);
                });
                button.name = m_ButtonName;
                button.transform.SetAsFirstSibling();
                button.transform.localPosition = Vector3.zero;
                button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                button.SetActive(true);

                //textMeshPro.SetTooltip(new TooltipTemplateSimple("test", "test2"));
                //var button = textMeshPro.gameObject.AddComponent<OwlcatButton>();
                //button.OnLeftClickAsObservable().Subscribe(_ =>
                //{
                //    Debug.Log("CLICK!");
                //});
                //button.OnHoverAsObservable().Subscribe(_ =>
                //{
                //    Debug.Log("HOVER!");
                //});

               // textMeshPro.HookupTextToSpeech();
            }
        }
    }
}
