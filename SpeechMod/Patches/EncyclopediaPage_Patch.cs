using HarmonyLib;
using Kingmaker;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Encyclopedia;
using SpeechMod.Unity;
using SpeechMod.Voice;
using System;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches
{

    [HarmonyPatch(typeof(EncyclopediaPagePCView), "UpdateView")]
    public static class EncyclopediaPage_Patch
    {
        private static readonly string m_ButtonName = "EncyclopediaSpeechButton";

        static void Postfix()
        {
            //try
            //{
                var bodyGroup = Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView/EncyclopediaPCView/EncyclopediaPageView/BodyGroup");
                if (bodyGroup == null)
                    return;
                
                var content = bodyGroup.Find("ObjectivesGroup/StandardScrollView/Viewport/Content");
                if (content == null)
                    return;
                
                var allTexts = content.gameObject.GetComponentsInChildren<TextMeshProUGUI>(true);
                if (allTexts == null || allTexts.Length == 0)
                    return;

                Debug.Log(allTexts.Length);

                foreach (var textMeshPro in allTexts)
                {
                    var parent = textMeshPro.transform;

                    var existingButton = parent.Find(m_ButtonName);
                    if (existingButton != null)
                        continue;

                    Debug.Log("Adding playbutton...");
                    var button = ButtonFactory.CreatePlayButton(parent, () =>
                    {
                        Speech.Speak(textMeshPro.text);
                    });
                    button.name = m_ButtonName;
                    button.transform.SetAsFirstSibling();
                    button.transform.localPosition = Vector3.zero;
                    button.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    button.gameObject.SetActive(true);
                }

                //foreach (var textMeshPro in allTexts)
                //{
                //    //textMeshPro.SetTooltip(new TooltipTemplateSimple("test", "test2"));
                //    //var button = textMeshPro.gameObject.AddComponent<OwlcatButton>();
                //    //button.OnLeftClickAsObservable().Subscribe(_ =>
                //    //{
                //    //    Debug.Log("CLICK!");
                //    //});
                //    //button.OnHoverAsObservable().Subscribe(_ =>
                //    //{
                //    //    Debug.Log("HOVER!");
                //    //});

                //    textMeshPro.HookupTextToSpeech();
                //}
            //}
            //catch (Exception ex)
            //{
            //    Debug.Log(ex.Message + ex.InnerException);
            //}
        }
    }
}
