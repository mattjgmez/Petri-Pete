using UnityEngine;
using System.Runtime.InteropServices; // For DLLImport
using System;
using System.IO.MemoryMappedFiles;
using System.IO;

public class ExternalWindowRenderer : MonoBehaviour
{
    // DLL Import
    [DllImport("SecondaryWindowPlugin")]
    private static extern IntPtr CreateNewWindow(string windowName, int width, int height);
    private IntPtr windowHandle;

    [DllImport("SecondaryWindowPlugin")]
    private static extern void DestroySecondaryWindow(IntPtr hwnd);

    [DllImport("SecondaryWindowPlugin")]
    private static extern void SendImageDataToSecondaryWindow(byte[] data, int length, int width, int height);

    private const string SHARED_MEMORY_NAME = "Local\\MySharedMemory";

    // Texture and Buffer
    public int DesiredWidth = 300;
    public int DesiredHeight = 300;
    public string WindowTitle = "New Window";
    private Texture2D sharedMemoryTexture;

    public Material displayMaterial;

    private MemoryMappedFile memoryMappedFile;
    private MemoryMappedViewAccessor accessor;

    private const int RETRY_COUNT = 3;
    private const int RETRY_DELAY = 1000; // milliseconds

    void Start()
    {
        sharedMemoryTexture = new Texture2D(DesiredWidth, DesiredHeight, TextureFormat.RGBA32, false);

        displayMaterial.mainTexture = sharedMemoryTexture;

        if (!InitializeSharedMemory())
        {
            Debug.LogError("Failed to initialize shared memory. Disabling ExternalWindowRenderer.", gameObject);
            this.enabled = false;
            return;
        }

        try
        {
            windowHandle = CreateNewWindow(WindowTitle, DesiredWidth, DesiredHeight);
            if (windowHandle == IntPtr.Zero)
            {
                Debug.LogError("Failed to create window or get its handle.", gameObject);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error when creating window: " + e.Message, gameObject);
        }
    }

    void Update()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(DesiredWidth, DesiredHeight, 24);
        Graphics.Blit(null, renderTexture);
        RenderTexture.active = renderTexture;

        sharedMemoryTexture.ReadPixels(new Rect(0, 0, DesiredWidth, DesiredHeight), 0, 0);
        sharedMemoryTexture.Apply();
        byte[] imageData = sharedMemoryTexture.GetRawTextureData();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        if (imageData != null && imageData.Length == DesiredWidth * DesiredHeight * 4)
        {
            SendImageDataToSecondaryWindow(imageData, imageData.Length, DesiredWidth, DesiredHeight);
        }
        else
        {
            Debug.LogError("Image data is invalid.", gameObject);
        }
    }

    void OnDestroy()
    {
        DestroySecondaryWindow(windowHandle);
        accessor?.Dispose();
        memoryMappedFile?.Dispose();
    }

    private bool InitializeSharedMemory()
    {
        for (int retry = 0; retry < RETRY_COUNT; retry++)
        {
            try
            {
                // Try to open the existing shared memory
                memoryMappedFile = MemoryMappedFile.OpenExisting(SHARED_MEMORY_NAME);

                // If successfully opened, break out of the loop
                break;
            }
            catch (FileNotFoundException)
            {
                // If it doesn't exist, create it
                long capacity = DesiredWidth * DesiredHeight * 4; // Assuming RGBA32 format
                memoryMappedFile = MemoryMappedFile.CreateNew(SHARED_MEMORY_NAME, capacity);
            }
            catch (Exception e)
            {
                // If any other exception occurs, log it
                Debug.LogError("Attempt " + (retry + 1) + ": Error initializing shared memory: " + e.Message, gameObject);

                // If this wasn't the last retry, wait before the next attempt
                if (retry < RETRY_COUNT - 1)
                {
                    System.Threading.Thread.Sleep(RETRY_DELAY);
                }
                else
                {
                    // If all retries failed, return false
                    return false;
                }
            }
        }

        try
        {
            accessor = memoryMappedFile.CreateViewAccessor();

            if (accessor == null)
            {
                Debug.LogError("Failed to create view accessor for shared memory.", gameObject);
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error creating view accessor: " + e.Message, gameObject);
            return false;
        }

        return true;
    }
}
