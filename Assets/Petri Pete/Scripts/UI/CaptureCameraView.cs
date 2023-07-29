using UnityEngine;
using System.Runtime.InteropServices;

public class CaptureCameraView : MonoBehaviour
{
    public Camera CaptureCamera;
    public int Width = 960;
    public int Height = 540;

    private RenderTexture rt;
    private Texture2D tex;

    // Declare external function for sending data to the secondary window
    [DllImport("SecondaryWindowPlugin")]
    private static extern void SendImageDataToSecondaryWindow(byte[] data, int length, int width, int height);

    void Awake()
    {
        CaptureCamera = GetComponent<Camera>();
        if (CaptureCamera == null)
        {
            Debug.LogError("Capture camera is not assigned!");
            return;
        }

        rt = new RenderTexture(Width, Height, 24);
        CaptureCamera.targetTexture = rt;

        tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        byte[] imageData = tex.GetRawTextureData();
        SendImageDataToSecondaryWindow(imageData, imageData.Length, rt.width, rt.height);
    }
}
