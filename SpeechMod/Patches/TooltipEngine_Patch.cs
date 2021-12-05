using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.Tooltip.Bricks;
using Kingmaker.UI.MVVM._VM.Tooltip.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SpeechMod
{
    [HarmonyPatch(typeof(TooltipEngine), "GetBrickView")]
    static class TooltipEngine_Patch
    {
        private static readonly string _buttonName = "TooltipSpeechButton";

        static void Postfix(ref MonoBehaviour __result)
        {
            if (!Main.Enabled)
                return;

            if (__result == null)
                return;

            //var logmessage = $"TooltipEngine, GetBrickView, Postfix, {__result.GetType().Name}";
            //Main.Logger.Log(logmessage);
            //Debug.Log(logmessage);

            if (__result is TooltipBrickTextView)
            {
                if (IsInvalid(__result.transform.parent)) // TODO: Better way of telling if inside temporary tooltip.
                    return;

                Debug.Log("Getting child 'Text (TMP)'...");

                var textMeshProTransform = __result.gameObject.transform.Find("Text (TMP)");
                if (textMeshProTransform == null)
                {
                    Debug.Log("No TextMeshProUGUI found!");
                    return;
                }

                Debug.Log("Getting component TextMeshPro");

                var textMeshPro = textMeshProTransform.GetComponent<TextMeshProUGUI>();
                if (textMeshPro == null)
                {
                    Debug.Log("No TextMeshProUGUI found!");
                    return;
                }

                //Debug.Log("Creating Speak button on Tooltip");
                //var infoView = Game.Instance.UI.FadeCanvas.transform.Find("InfoWindowPCViewBig");
                //if (infoView == null)
                //{
                //    Debug.LogWarning("No InfoWindowPCViewBig found!");
                //    return;
                //}

                //var content = infoView.Find("Window/BodyContainer/ScrollView/ViewPort/Content");
                //if (content == null)
                //{
                //    Debug.LogWarning("No 'Content' found!");
                //    return;
                //}

                Debug.Log("Getting existsing button...");
                try
                {
                    var existingButton = __result.transform.Find($"_canvas_{_buttonName}");

                    if (existingButton != null)
                    {
                        Debug.Log($"{_buttonName} already found!");
                        return;
                    }
                }
                catch { } // Eat it!

                //var existingButton = GetButton(content, __result as TooltipBrickTextView);
                //if (existingButton != null)
                //{
                //    Debug.Log($"{_buttonName} already found!");
                //    existingButton.SetActive(true);
                //    return;
                //}

                var canvas = new GameObject($"_canvas_{_buttonName}");
                var hlg = canvas.AddComponent<Kingmaker.UI.UICanvas>();
                
                var buttonGameObject = ButtonFactory.CreatePlayButton(canvas.transform, () =>
                {
                    Speech.Speak(textMeshPro.text);
                });
                buttonGameObject.name = _buttonName;
                buttonGameObject.transform.localScale = Vector3.one;
                buttonGameObject.transform.localPosition = Vector3.zero;
                buttonGameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                //buttonGameObject.AddComponent<TooltipButton>().tooltipBrick = (TooltipBrickTextView)__result;
                buttonGameObject.SetActive(true);
                canvas.transform.SetParent(__result.transform);
                canvas.transform.localScale = Vector3.one;
                canvas.transform.localPosition = Vector3.zero;
                canvas.SetActive(true);
            }
        }

        private static bool IsInvalid(Transform parent)
        {
            return
                parent == null ||
                parent.name.Equals("HeaderContainer") ||
                parent.name.Equals("FooterContainer");
        }

        private static GameObject GetButton(Transform content, TooltipBrickTextView tooltipBrickTextView)
        {
            var buttons = content.GetComponentsInChildren<TooltipButton>(true);
            GameObject theButton = null;
            foreach (var button in buttons)
            {
                if (button.Equals(tooltipBrickTextView))
                    theButton = button.gameObject;
                else
                    button.gameObject.SetActive(tooltipBrickTextView.gameObject.activeSelf);
            }
            return theButton;
        }

    }
}
