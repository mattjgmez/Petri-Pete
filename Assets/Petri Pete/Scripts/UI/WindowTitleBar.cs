using UnityEngine;
using UnityEngine.EventSystems;

public class WindowTitleBar : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform Window;

    private Canvas canvas;
    private Vector2 offset;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (!canvas)
        {
            Debug.LogWarning("WindowTitleBar is not a child of a GameObject with a Canvas component!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(Window, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pointerPostion = ClampToWindow(eventData);

        Window.anchoredPosition = pointerPostion - offset;
    }

    private Vector2 ClampToWindow(PointerEventData eventData)
    {
        Vector2 rawPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out rawPointerPosition);
        return rawPointerPosition;
    }
}
