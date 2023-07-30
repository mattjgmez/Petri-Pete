using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowController : MonoBehaviour
{
    public TMP_Text TitleBarText;
    public TMP_Text MinimizeBarText;

    public GameObject MainContent;
    public RenderTexture RenderTexture;
    public GameObject WindowPanel;
    public GameObject MinimizeBar;

    public Vector2 MaximizePosition;

    protected RawImage _imageDisplay;

    private void Start()
    {
        // Start with everything in its maximized state
        MainContent.SetActive(true);
        WindowPanel.SetActive(true);

        MinimizeBar.SetActive(false);

        string title = RenderTexture.name;
        TitleBarText.text = title;
        MinimizeBarText.text = title;

        _imageDisplay = GetComponentInChildren<RawImage>();

        UpdateRenderTexture();
        _imageDisplay.texture = RenderTexture;

        transform.position = MaximizePosition;
    }

    public void ToggleWindow()
    {
        bool isActive = MainContent.activeSelf;
        MainContent.SetActive(!isActive);
        WindowPanel.SetActive(!isActive);

        MinimizeBar.SetActive(isActive);
    }

    public void UpdateRenderTexture()
    {
        if (_imageDisplay != null)
        {
            _imageDisplay.texture = RenderTexture;
        }
        else
        {
            Debug.LogWarning("RawImage reference is not set in the WindowController.");
        }
    }

    public void OnDrawGizmosSelected()
    {
        // Conversion factor
        float ppu = 100f; // For UI, this is typically 100 pixels per unit.

        Vector2 sizeMaximize = new Vector2(((RectTransform)transform).rect.width, ((RectTransform)transform).rect.height) / ppu;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(MaximizePosition, sizeMaximize);
    }
}
