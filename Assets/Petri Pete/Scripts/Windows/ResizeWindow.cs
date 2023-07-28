using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The ResizeWindow class allows a UI window to be resized while in full-screen mode by dragging its edges.
/// It inherits from DraggableWindow to handle dragging the window, and also implements IPointerUpHandler to check for resizing actions.
/// </summary>
public class ResizeWindow : DraggableWindow, IPointerUpHandler
{
    public bool KeepSquare = false;

    protected bool _didResizing = false;
    protected const float _windowMinimum = 400f;

    /// <summary>
    /// Handles the drag event to resize the UI window while in full-screen mode or pass the event to the base class for regular dragging.
    /// </summary>
    /// <param name="eventData">The PointerEventData associated with the event.</param>
    public override void OnDrag(PointerEventData eventData)
    {
        // Check if the UI window is in full-screen mode
        if (ToggleFullscreenOnClick.IsFullscreened)
        {
            // If the UI window was not dragged before, set the resizing flag
            if (!WasDragged)
            {
                WasDragged = true;
                _didResizing = true;
            }

            // Calculate the new size based on the drag amount and clamp it within the canvas boundaries
            Vector2 dragDelta = eventData.delta;
            float clampedX = Mathf.Clamp(dragDelta.x + _rectTransform.sizeDelta.x, _windowMinimum, _halfWidth * 2);
            float clampedY = Mathf.Clamp(dragDelta.y + _rectTransform.sizeDelta.y, _windowMinimum, _halfHeight * 2);
            Vector2 newSize = new Vector2(clampedX, clampedY);

            // Resize the UI window
            _rectTransform.sizeDelta = newSize;

            // Resize the children as well to maintain the square aspect ratio
            ResizeChildren(newSize);
        }
        else
        {
            // If not in full-screen mode, handle dragging using the base class implementation
            _didResizing = false;
            base.OnDrag(eventData);
        }
    }

    /// <summary>
    /// Handles the pointer up event to check for resizing actions and reset the resizing flag.
    /// </summary>
    /// <param name="eventData">The PointerEventData associated with the event.</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // Check if the window was resized, and set the full-screen flag accordingly
        if (_didResizing)
        {
            ToggleFullscreenOnClick fullscreenToggle = GetComponent<ToggleFullscreenOnClick>();

            ToggleFullscreenOnClick.IsFullscreened = false;
            fullscreenToggle.OriginalSize = _rectTransform.sizeDelta;
            fullscreenToggle.OriginalChildSize = fullscreenToggle.GetChildSize();
        }
        _didResizing = false;
    }

    /// <summary>
    /// Resizes the child elements to maintain a square aspect ratio based on the new size of the parent.
    /// </summary>
    /// <param name="parentSize">The new size of the parent UI element.</param>
    private void ResizeChildren(Vector2 parentSize)
    {
        for (int i = 0; i < _rectTransform.childCount; i++)
        {
            RectTransform childRectTransform = _rectTransform.GetChild(i) as RectTransform;
            if (childRectTransform != null)
            {
                // Calculate the new size for children based on the parent's new size and maintain square aspect ratio
                float minSize = Mathf.Min(parentSize.x, parentSize.y);

                // Apply the new size to the child
                childRectTransform.sizeDelta = KeepSquare ? new Vector2(minSize, minSize) : parentSize;
            }
        }
    }
}
