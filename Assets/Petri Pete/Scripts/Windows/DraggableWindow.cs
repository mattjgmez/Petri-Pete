using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public static bool WasDragged = false;

    protected RectTransform _rectTransform;
    protected Vector2 _offset;
    protected Vector2 _canvasSize;
    protected float _halfWidth;
    protected float _halfHeight;
    protected float _elementWidth;
    protected float _elementHeight;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // Get the canvas size
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            _canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
            _halfWidth = _canvasSize.x * 0.5f;
            _halfHeight = _canvasSize.y * 0.5f;
        }

        // Get half of the UI element's size for clamping
        _elementWidth = _rectTransform.sizeDelta.x;
        _elementHeight = _rectTransform.sizeDelta.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WasDragged = false;

        // Calculate the offset between the clicked point and the UI element's position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _offset);

        _rectTransform.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!WasDragged)
        {
            WasDragged = true;
        }
        if (ToggleFullscreenOnClick.IsFullscreened)
        {
            return;
        }

        // Move the UI element to the new position based on the input
        Vector2 newPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out newPosition))
        {
            // Calculate the clamped position
            float clampedX = Mathf.Clamp(newPosition.x, -_halfWidth + _offset.x , _halfWidth + _offset.x - _elementWidth);
            float clampedY = Mathf.Clamp(newPosition.y, -_halfHeight + _offset.y , _halfHeight + _offset.y - _elementHeight);

            _rectTransform.localPosition = new Vector3(clampedX - _offset.x, clampedY - _offset.y, _rectTransform.localPosition.z);
        }
    }
}
