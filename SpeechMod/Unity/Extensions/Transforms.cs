using UnityEngine;

namespace SpeechMod.Unity.Extensions;

public static class Transforms
{
    //------------Top-------------------
    public static void RectAlignTopLeft(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0, 1);
        var anchorMax = new Vector2(0, 1);
        var pivot = new Vector2(0, 1);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignTopMiddle(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0.5f, 1);
        var anchorMax = new Vector2(0.5f, 1);
        var pivot = new Vector2(0.5f, 1);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignTopRight(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(1, 1);
        var anchorMax = new Vector2(1, 1);
        var pivot = new Vector2(1, 1);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    //------------Middle-------------------
    public static void RectAlignMiddleLeft(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0, 0.5f);
        var anchorMax = new Vector2(0, 0.5f);
        var pivot = new Vector2(0, 0.5f);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignMiddle(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0.5f, 0.5f);
        var anchorMax = new Vector2(0.5f, 0.5f);
        var pivot = new Vector2(0.5f, 0.5f);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignMiddleRight(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(1, 0.5f);
        var anchorMax = new Vector2(1, 0.5f);
        var pivot = new Vector2(1, 0.5f);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    //------------Bottom-------------------
    public static void RectAlignBottomLeft(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0, 0);
        var anchorMax = new Vector2(0, 0);
        var pivot = new Vector2(0, 0);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignBottomMiddle(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(0.5f, 0);
        var anchorMax = new Vector2(0.5f, 0);
        var pivot = new Vector2(0.5f, 0);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    public static void RectAlignBottomRight(this GameObject uiObject, Vector2? anchoredPosition = null)
    {
        var anchorMin = new Vector2(1, 0);
        var anchorMax = new Vector2(1, 0);
        var pivot = new Vector2(1, 0);

        SetRectAlign(uiObject, anchorMin, anchorMax, pivot, anchoredPosition);
    }

    private static void SetRectAlign(GameObject uiObject, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2? anchoredPosition = null)
    {
        var uitransform = uiObject.GetComponent<RectTransform>();

        if (uitransform == null)
            return;

        uitransform.anchorMin = anchorMin;
        uitransform.anchorMax = anchorMax;
        uitransform.pivot = pivot;

        if (anchoredPosition.HasValue)
            uitransform.anchoredPosition = anchoredPosition.Value;
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
        var oldSize = trans.rect.size;
        var deltaSize = newSize - oldSize;
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
}