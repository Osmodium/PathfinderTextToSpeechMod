using System;
using System.Collections;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SpeechMod.Unity;

public static class Extensions
{
    private static Color m_HoverColor = Color.blue;

    public static Coroutine ExecuteLater(this MonoBehaviour behaviour, float delay, Action action)
    {
        return behaviour.StartCoroutine(_realExecute(delay, action));
    }

    static IEnumerator _realExecute(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public static void UpdateHoverColor()
    {
        m_HoverColor = new Color(Main.Settings.HoverColorR, Main.Settings.HoverColorG, Main.Settings.HoverColorB, Main.Settings.HoverColorA);
    }

    public static Transform GetParentRecursive(this Transform transform, string name)
    {
        if (transform?.parent == null)
            return null;

        if (transform.parent.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            return transform.parent;

        return transform.parent.GetParentRecursive(name);
    }

    public static void HookupTextToSpeechOnTransform(this Transform transform)
    {
        if (transform == null)
        {
            Debug.LogWarning("Can't hook up text to speech on null transform!");
            return;
        }

        var allTexts = transform.GetComponentsInChildren<TextMeshProUGUI>();
        allTexts.HookupTextToSpeech();
    }

    public static void HookupTextToSpeech(this TextMeshProUGUI[] textMeshPros)
    {
        foreach (var textMeshPro in textMeshPros)
        {
            textMeshPro.HookupTextToSpeech();
        }
    }

    public static void HookupTextToSpeech(this TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("No TextMeshProUGUI!");
            return;
        }

        var textMeshProTransform = textMeshPro.transform;
        if (textMeshProTransform == null)
        {
            Debug.LogWarning("Transform on TextMeshProUGUI is null!");
            return;
        }

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

        textMeshPro.OnPointerEnterAsObservable().Subscribe(
            _ =>
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
                    textMeshPro.color = m_HoverColor;
            }
        );

        textMeshPro.OnPointerExitAsObservable().Subscribe(
            _ =>
            {
                textMeshPro.fontStyle = defaultValues.FontStyles;
                textMeshPro.color = defaultValues.Color;
            }
        );

        textMeshPro.OnPointerClickAsObservable().Subscribe(
            clickEvent =>
            {
                if (clickEvent.button == UnityEngine.EventSystems.PointerEventData.InputButton.Left)
                    Main.Speech.Speak(textMeshPro.text);
            }
        );
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

    public static void SetDefaultScale(this RectTransform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
    }
    public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
    {
        trans.pivot = aVec;
        trans.anchorMin = aVec;
        trans.anchorMax = aVec;
    }

    public static Vector2 GetSize(this RectTransform trans)
    {
        return trans.rect.size;
    }
    public static float GetWidth(this RectTransform trans)
    {
        return trans.rect.width;
    }
    public static float GetHeight(this RectTransform trans)
    {
        return trans.rect.height;
    }

    public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }

    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }

    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSize(this RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin -= new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax += new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }

    public static void SetWidth(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(newSize, trans.rect.size.y));
    }

    public static void SetHeight(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(trans.rect.size.x, newSize));
    }

    public static Transform TryFind(this Transform transform, string n)
    {
        if (string.IsNullOrWhiteSpace(n) || transform == null)
            return null;

        try
        {
            return transform.Find(n);
        }
        catch
        {
            Debug.Log("TryFind found nothing!");
        }

        return null;
    }
}