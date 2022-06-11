using DG.Tweening;
using Kingmaker.UI.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SpeechMod.Unity;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
    private bool m_MoveMode;
    private Vector2 m_MouseStartPos;
    private Vector2 m_ContainerStartPos;
    private Vector2 m_LastMousePos;
    private Vector2 m_TakeDrag;
    private RectTransform m_OwnRectTransform;
    private RectTransform m_ParentRectTransform;
    
    
    private void Start()
    {
        m_TakeDrag = new Vector2(0f, 0f);
        m_OwnRectTransform = (RectTransform)transform.parent;
        m_ParentRectTransform = (RectTransform)m_OwnRectTransform.parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        
        m_MoveMode = true;
        m_MouseStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        m_OwnRectTransform.anchoredPosition += m_TakeDrag;
        m_OwnRectTransform.DOAnchorPos(m_OwnRectTransform.anchoredPosition + m_TakeDrag, 0.1f).SetUpdate(true);
        m_ContainerStartPos = m_OwnRectTransform.anchoredPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_OwnRectTransform.DOAnchorPos(m_OwnRectTransform.anchoredPosition - m_TakeDrag, 0.1f).SetUpdate(true);
        m_MoveMode = false;
        m_MouseStartPos = default;
    }

    public void LateUpdate()
    {
        if (!m_MoveMode)
            return;
        Vector2 vector2 = new Vector2(Input.mousePosition.x - m_MouseStartPos.x, Input.mousePosition.y - m_MouseStartPos.y);
        if (m_LastMousePos == vector2)
            return;
        m_OwnRectTransform.anchoredPosition = UIUtility.LimitPositionRectInRect(m_ContainerStartPos + vector2 - m_TakeDrag, m_ParentRectTransform, m_OwnRectTransform) + m_TakeDrag;
        m_LastMousePos = vector2;
    }
}
