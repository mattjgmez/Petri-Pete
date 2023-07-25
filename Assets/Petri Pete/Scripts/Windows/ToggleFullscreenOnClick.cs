using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The ToggleFullscreenOnClick class enables a UI element to toggle between full-screen and original size when clicked.
/// It automatically adjusts the size of the UI element and its children to maintain a square aspect ratio during the transition.
/// </summary>
public class ToggleFullscreenOnClick : MonoBehaviour
{
    public static bool IsFullscreened = false;

    protected RectTransform _rectTransform;
    protected Vector2 _originalSize;
    protected Vector2 _originalPosition;
    protected Vector2 _originalChildSize;


    /// <summary>
    /// Performs the initial setup by finding the RectTransform component and storing original values.
    /// </summary>
    protected virtual void Awake()
    {
        Initialization();
    }

    /// <summary>
    /// Initializes the ToggleFullscreenOnClick component by locating the RectTransform and storing its original size and position.
    /// </summary>
    protected virtual void Initialization()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalSize = _rectTransform.sizeDelta;
        _originalPosition = _rectTransform.anchoredPosition;

        // Store the original size of the children
        _originalChildSize = GetChildSize();
    }

    /// <summary>
    /// Toggles the size of the UI element and its children between full-screen and original size when clicked.
    /// </summary>
    public void ToggleSize()
    {
        if (DraggableWindow.WasDragged)
        {
            return;
        }
        if (IsFullscreened)
        {
            // Return to the original size and position
            _rectTransform.sizeDelta = _originalSize;
            _rectTransform.anchoredPosition = _originalPosition;

            // Restore the original size of the children
            SetChildSize(_originalChildSize);
        }
        else
        {
            // Make the UI element full-screen (use canvas size) and reset its position
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                _originalPosition = _rectTransform.anchoredPosition;

                _rectTransform.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
                _rectTransform.anchoredPosition = Vector2.zero;
                _rectTransform.transform.SetAsLastSibling(); // Brings the UI element to the front.

                // Calculate the square size for children and apply it
                Vector2 squareSize = CalculateSquareSize();
                SetChildSize(squareSize);
            }
        }

        // Toggle the state
        IsFullscreened = !IsFullscreened;
    }

    /// <summary>
    /// Calculates the square size to maintain the aspect ratio of the UI element during resizing.
    /// </summary>
    /// <returns>The square size vector (same width and height).</returns>
    protected virtual Vector2 CalculateSquareSize()
    {
        float minSize = Mathf.Min(_rectTransform.sizeDelta.x, _rectTransform.sizeDelta.y);
        return new Vector2(minSize, minSize);
    }

    /// <summary>
    /// Retrieves the size of the first child RectTransform found within the UI element.
    /// </summary>
    /// <returns>The size of the first child RectTransform.</returns>
    protected virtual Vector2 GetChildSize()
    {
        Vector2 childSize = Vector2.zero;
        for (int i = 0; i < _rectTransform.childCount; i++)
        {
            RectTransform childRectTransform = _rectTransform.GetChild(i) as RectTransform;
            if (childRectTransform != null)
            {
                childSize = childRectTransform.sizeDelta;
                break; // Assuming all children have the same size
            }
        }
        return childSize;
    }

    /// <summary>
    /// Sets the size of all child RectTransforms to the specified size.
    /// </summary>
    /// <param name="size">The size to set for all child RectTransforms.</param>
    protected virtual void SetChildSize(Vector2 size)
    {
        for (int i = 0; i < _rectTransform.childCount; i++)
        {
            RectTransform childRectTransform = _rectTransform.GetChild(i) as RectTransform;
            if (childRectTransform != null)
            {
                childRectTransform.sizeDelta = size;
            }
        }
    }
}
