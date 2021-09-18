using System.Speech.Synthesis;
using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Owlcat.Runtime.UI.Controls.Button;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod
{
    [HarmonyPatch(typeof(StaticCanvas), "Initialize")]
    static class DialogCurrentPart_Patch
    {
        private static bool m_Initialized;

        //static void Prefix(ref CueShowData __cueData)
        //{
        //    if (!Main.Enabled) 
        //        return;

        //}

        static void Postfix()
        {
            if (!Main.Enabled || m_Initialized)
                return;

            m_Initialized = true;
            
            Debug.Log("Speech Testing");

            var parent = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View");
            Debug.Log("1");
            var referenceBackGround = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View/ButtonEdge").GetComponent<Image>();
            Debug.Log("2");
            var referenceArrow = Game.Instance.UI.Canvas.transform.Find("DialogPCView/Body/View/Scroll View/ButtonEdge/Image").GetComponent<Image>();
            Debug.Log("3");
            
            Debug.Log(referenceBackGround.name);
            Debug.Log(referenceArrow.name);
            
            //var referenceArrowImage = referenceArrow.GetComponent<Image>();
            //Image referenceImage = referenceButton.GetComponent<Image>();
            //Sprite backgroundSprite = Sprite.Create(referenceImage.sprite.texture, referenceImage.GetPixelAdjustedRect(), new Vector2(.5f, .5f));

            GameObject buttonGameObject = new GameObject("SpeechButton");
            buttonGameObject.transform.SetParent(parent);
            buttonGameObject.transform.localPosition = new Vector3(-493, 164, 0);
            buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            buttonGameObject.transform.localScale = Vector3.one;

            var backgroundImage = buttonGameObject.AddComponent<Image>();
            backgroundImage.sprite = referenceBackGround.sprite;
            
            OwlcatButton speechButton = buttonGameObject.AddComponent<OwlcatButton>();
            speechButton.OnLeftClick.AddListener(Call);
            
            GameObject arrowGameObject = new GameObject("Image");
            arrowGameObject.transform.SetParent(buttonGameObject.transform);
            arrowGameObject.transform.localPosition = Vector3.zero;
            arrowGameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            
            var arrowImage = arrowGameObject.AddComponent<Image>();
            arrowImage.sprite = referenceArrow.sprite;
        }

        private static void Call()
        {
            new SpeechSynthesizer().SpeakAsync(Game.Instance.DialogController.CurrentCue.DisplayText);
        }
    }

    //[HarmonyPatch(typeof(DialogCurrentPart), "Show")]
    //public static class DialogCurrentPart_Show_Patch
    //{
    //    static void Postfix()
    //    {
            
    //        if (GUILayout.Button("Speak", GUILayout.ExpandWidth(false)))
    //        {
    //            new SpeechSynthesizer().Speak(Game.Instance.DialogController.CurrentCue.DisplayText);
    //        }
    //    }
    //}
}
