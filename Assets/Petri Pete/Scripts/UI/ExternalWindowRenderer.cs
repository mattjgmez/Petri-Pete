using UnityEngine;
using System.Runtime.InteropServices; // For DLLImport
using System;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Linq;

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

    // Settings
    public int DesiredWidth = 300;
    public int DesiredHeight = 300;
    public string WindowTitle = "New Window";

    public RenderTexture cameraRenderTexture; // Assign this from Unity editor

    private MemoryMappedFile memoryMappedFile;
    private MemoryMappedViewAccessor accessor;

    private const int RETRY_COUNT = 3;
    private const int RETRY_DELAY = 1000; // milliseconds

    void Start()
    {
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
        Texture2D texture2D = RenderTextureToTexture2D(cameraRenderTexture);
        byte[] imageData = texture2D.GetRawTextureData();

        if (imageData != null && imageData.Length == DesiredWidth * DesiredHeight * 4)
        {
            byte[] widthBytes = BitConverter.GetBytes(DesiredWidth);
            byte[] heightBytes = BitConverter.GetBytes(DesiredHeight);

            accessor.WriteArray<byte>(0, widthBytes, 0, widthBytes.Length);
            accessor.WriteArray<byte>(4, heightBytes, 0, heightBytes.Length);
            accessor.WriteArray<byte>(8, imageData, 0, imageData.Length);

#if UNITY_EDITOR
            byte[] writtenWidthBytes = new byte[4];
            byte[] writtenHeightBytes = new byte[4];
            byte[] writtenImageData = new byte[DesiredWidth * DesiredHeight * 4];

            accessor.ReadArray<byte>(0, writtenWidthBytes, 0, writtenWidthBytes.Length);
            accessor.ReadArray<byte>(4, writtenHeightBytes, 0, writtenHeightBytes.Length);
            accessor.ReadArray<byte>(8, writtenImageData, 0, writtenImageData.Length);

            if (!widthBytes.SequenceEqual(writtenWidthBytes) 
            || !heightBytes.SequenceEqual(writtenHeightBytes) 
            || !imageData.SequenceEqual(writtenImageData))
            {
                Debug.LogError($"{this.GetType()}.Update: Written data in shared memory does not match the original data.", gameObject);
            }
#endif

            // Additional logging for debugging purposes
            Debug.Log($"{this.GetType()}.Update: Successfully wrote image data to shared memory.", gameObject);

#if UNITY_EDITOR
            // Save the image for visual validation
            byte[] pngBytes = texture2D.EncodeToPNG();
            string path = System.IO.Path.Combine(Application.persistentDataPath, "debugImage.png");
            System.IO.File.WriteAllBytes(path, pngBytes);
            Debug.Log($"{this.GetType()}.Update: Image saved to: {path}", gameObject);
#endif
        }
        else
        {
            Debug.LogError($"{this.GetType()}.Update: Image data is invalid. Length: {imageData?.Length}, Expected: {DesiredWidth * DesiredHeight * 4}", gameObject);
        }

        Destroy(texture2D); // Cleanup to prevent memory leaks
    }

    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        return texture2D;
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
                // Calculate capacity with 8 bytes added for width and height storage.
                long capacity = 8 + (DesiredWidth * DesiredHeight * 4); // Assuming RGBA32 format
                Debug.Log($"{this.GetType()}.InitializeSharedMemory: Calculating capacity. Capacity = {capacity}.", gameObject);

                // Check if shared memory exists, if not create one
                memoryMappedFile = MemoryMappedFile.CreateOrOpen(SHARED_MEMORY_NAME, capacity);

                if (memoryMappedFile == null)
                {
                    Debug.LogError($"{this.GetType()}.InitializeSharedMemory: memoryMappedFile is null after trying to open existing shared memory.", gameObject);
                }

                // If successfully opened, break out of the loop
                break;
            }
            catch (Exception e)
            {
                // If any exception occurs, log it
                Debug.LogError($"{this.GetType()}.InitializeSharedMemory: Attempt " + (retry + 1) + $": Error initializing shared memory: {e.Message}", gameObject);

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
                Debug.LogError($"{this.GetType()}.InitializeSharedMemory: Failed to create view accessor for shared memory.", gameObject);
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{this.GetType()}.InitializeSharedMemory: Error creating view accessor: " + e.Message, gameObject);
            return false;
        }

        Debug.Log($"{this.GetType()}.InitializeSharedMemory: Shared Memory successfully Initialized after {RETRY_COUNT + 1} total attempts.", gameObject);
        return true;
    }
}
