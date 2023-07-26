using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The DraggableWindow class allows a UI window to be draggable within the boundaries of the canvas.
/// It responds to user input for dragging and ensures the window stays within the visible area of the canvas.
/// </summary>
public class DraggableWindow : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    /// <summary>
    /// Gets or sets a value indicating whether the window was dragged since the last check.
    /// </summary>
    public static bool WasDragged = false;

    protected RectTransform _rectTransform;
    protected Vector2 _offset;
    protected Vector2 _canvasSize;
    protected float _halfWidth;
    protected float _halfHeight;

    /// <summary>
    /// Performs initial setup and calculates necessary values.
    /// </summary>
    protected virtual void Awake()
    {
        Initialization();
    }

    /// <summary>
    /// Initializes the DraggableWindow component by finding the RectTransform and calculating canvas-related values.
    /// </summary>
    protected virtual void Initialization()
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
    }

    /// <summary>
    /// Handles the pointer down event to start the dragging process.
    /// </summary>
    /// <param name="eventData">The PointerEventData associated with the event.</param>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        WasDragged = false;

        // Calculate the offset between the clicked point and the UI element's position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _offset);

        // Bring the UI window to the front within its parent
        _rectTransform.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Handles the drag event to move the UI window within the canvas boundaries.
    /// </summary>
    /// <param name="eventData">The PointerEventData associated with the event.</param>
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!WasDragged)
        {
            WasDragged = true;
        }

        // Check if the UI window is in full-screen mode, prevent dragging if true
        if (ToggleFullscreenOnClick.IsFullscreened)
        {
            return;
        }

        // Move the UI window to the new position based on the input
        Vector2 newPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, eventData.position, eventData.pressEventCamera, out newPosition))
        {
            // Calculate the clamped position within the canvas boundaries
            float clampedX = Mathf.Clamp(newPosition.x, -_halfWidth + _offset.x, _halfWidth + _offset.x - _rectTransform.sizeDelta.x);
            float clampedY = Mathf.Clamp(newPosition.y, -_halfHeight + _offset.y, _halfHeight + _offset.y - _rectTransform.sizeDelta.y);

            _rectTransform.localPosition = new Vector3(clampedX - _offset.x, clampedY - _offset.y, _rectTransform.localPosition.z);
        }
    }
}
