﻿using SpeechMod.Voice;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SpeechMod.Unity
{
    public static class Extensions
    {
        public static void HookupTextToSpeech(this TextMeshProUGUI textMeshPro)
        {
            if (textMeshPro == null)
                return;

            var textMeshProTransform = textMeshPro.transform;
            var skipEventAssignment = false;

            var defaultValues = textMeshProTransform.GetComponent<TextMeshProValues>();
            if (defaultValues == null)
                defaultValues = textMeshProTransform.gameObject?.AddComponent<TextMeshProValues>();
            else
                skipEventAssignment = true;
            
            defaultValues.FontStyles = textMeshPro.fontStyle;
            defaultValues.Color = textMeshPro.color;

            if (skipEventAssignment)
            {
#if DEBUG
                Debug.Log("Skipping event assignment!");
#endif
                return;
            }

            textMeshPro.OnPointerEnterAsObservable().Subscribe(_ =>
            {
                if (Main.Settings.FontStyleOnHover)
                {
                    for (int i = 0; i < Main.Settings.FontStyles.Length; i++)
                    {
                        if (Main.Settings.FontStyles[i])
                            textMeshPro.fontStyle |= (FontStyles)Enum.Parse(typeof(FontStyles), Main.FontStyleNames[i], true);
                    }
                }

                if (Main.Settings.ColorOnHover)
                    textMeshPro.color = Main.ChosenColor;
            });

            textMeshPro.OnPointerExitAsObservable().Subscribe(_ =>
            {
                textMeshPro.fontStyle = defaultValues.FontStyles;
                textMeshPro.color = defaultValues.Color;
            });

            textMeshPro.OnPointerClickAsObservable().Subscribe(clickEvent =>
            {
                if (clickEvent.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left)
                    Speech.Speak(textMeshPro.text);
            });
        }

        //------------Top-------------------
        public static void RectAlignTopLeft(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0, 1);
            var anchorMax = new Vector2(0, 1);
            var pivot = new Vector2(0, 1);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignTopMiddle(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0.5f, 1);
            var anchorMax = new Vector2(0.5f, 1);
            var pivot = new Vector2(0.5f, 1);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignTopRight(this GameObject uiObject)
        {
            var anchorMin = new Vector2(1, 1);
            var anchorMax = new Vector2(1, 1);
            var pivot = new Vector2(1, 1);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        //------------Middle-------------------
        public static void RectAlignMiddleLeft(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0, 0.5f);
            var anchorMax = new Vector2(0, 0.5f);
            var pivot = new Vector2(0, 0.5f);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignMiddle(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0.5f, 0.5f);
            var anchorMax = new Vector2(0.5f, 0.5f);
            var pivot = new Vector2(0.5f, 0.5f);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignMiddleRight(this GameObject uiObject)
        {
            var anchorMin = new Vector2(1, 0.5f);
            var anchorMax = new Vector2(1, 0.5f);
            var pivot = new Vector2(1, 0.5f);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        //------------Bottom-------------------
        public static void RectAlignBottomLeft(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0, 0);
            var anchorMax = new Vector2(0, 0);
            var pivot = new Vector2(0, 0);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignBottomMiddle(this GameObject uiObject)
        {
            var anchorMin = new Vector2(0.5f, 0);
            var anchorMax = new Vector2(0.5f, 0);
            var pivot = new Vector2(0.5f, 0);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        public static void RectAlignBottomRight(this GameObject uiObject)
        {
            var anchorMin = new Vector2(1, 0);
            var anchorMax = new Vector2(1, 0);
            var pivot = new Vector2(1, 0);

            SetRectAlign(uiObject, anchorMin, anchorMax, pivot);
        }

        private static void SetRectAlign(GameObject uiObject, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            if (uitransform == null)
                return;

            uitransform.anchorMin = anchorMin;
            uitransform.anchorMax = anchorMax;
            uitransform.pivot = pivot;
        }
    }
}