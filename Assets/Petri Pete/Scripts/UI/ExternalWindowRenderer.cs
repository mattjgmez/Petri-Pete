using UnityEngine;
using System.Runtime.InteropServices; // For DLLImport
using System;
using System.IO.MemoryMappedFiles;
using System.IO;

[RequireComponent(typeof(CaptureCameraView))]
public class ExternalWindowRenderer : MonoBehaviour
{
    // DLL Import
    [DllImport("SecondaryWindowPlugin")]
    private static extern IntPtr CreateNewWindow(string windowName, int width, int height);
    private IntPtr windowHandle;  // Variable to store the window handle

    [DllImport("SecondaryWindowPlugin")]
    private static extern void DestroySecondaryWindow(IntPtr hwnd);

    [DllImport("SecondaryWindowPlugin")]
    private static extern void SendImageDataToSecondaryWindow(byte[] data, int length, int width, int height);

    private const string SHARED_MEMORY_NAME = "Local\\MySharedMemory";

    // Texture and Buffer
    public CaptureCameraView CameraView;
    public int DesiredWidth = 1920;  // Adjust as needed
    public int DesiredHeight = 1080; // Adjust as needed
    public string WindowTitle = "New Window";
    private Texture2D sharedMemoryTexture;
    private byte[] textureBuffer;

    public Material displayMaterial; // Assign in inspector

    private MemoryMappedFile memoryMappedFile;
    private MemoryMappedViewAccessor accessor;

    void Start()
    {
        // Initialize window resolution
        CameraView = GetComponent<CaptureCameraView>();
        DesiredWidth = CameraView.Width; 
        DesiredHeight = CameraView.Height;

        // Initialize texture and buffer
        sharedMemoryTexture = new Texture2D(DesiredWidth, DesiredHeight, TextureFormat.RGBA32, false);
        textureBuffer = new byte[DesiredWidth * DesiredHeight * 4];

        displayMaterial.mainTexture = sharedMemoryTexture;

        // Initialize shared memory
        if (!InitializeSharedMemory())
        {
            Debug.LogError("Failed to initialize shared memory. Disabling ExternalWindowRenderer.");
            this.enabled = false; // Disable this component
            return;
        }

        // Assuming you want to create a new window on start:
        try
        {
            windowHandle = CreateNewWindow(WindowTitle, DesiredWidth, DesiredHeight);
            if (windowHandle == IntPtr.Zero)
            {
                Debug.LogError("Failed to create window or get its handle.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error when creating window: " + e.Message);
        }
    }

    void Update()
    {
        // Update texture with shared memory data
        ReadSharedMemoryIntoBuffer(textureBuffer);

        sharedMemoryTexture.LoadRawTextureData(textureBuffer);
        sharedMemoryTexture.Apply();

        // Assuming you want to send this texture data to the shared memory every frame
        byte[] imageData = sharedMemoryTexture.GetRawTextureData();
        //SendImageDataToSecondaryWindow(imageData, imageData.Length, DesiredWidth, DesiredHeight);
    }

    void OnDestroy()
    {
        DestroySecondaryWindow(windowHandle);

        accessor?.Dispose();
        memoryMappedFile?.Dispose();
    }

    private bool InitializeSharedMemory()
    {
        try
        {
            memoryMappedFile = MemoryMappedFile.OpenExisting(SHARED_MEMORY_NAME);
            accessor = memoryMappedFile.CreateViewAccessor();

            if (accessor == null)
            {
                Debug.LogError("Failed to create view accessor for shared memory.");
                return false;
            }
            return true;
        }
        catch (FileNotFoundException)
        {
            Debug.LogError($"MemoryMappedFile '{SHARED_MEMORY_NAME}' does not exist.");
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError("Error initializing shared memory: " + e.Message);
            return false;
        }
    }

    private void ReadSharedMemoryIntoBuffer(byte[] buffer)
    {
        if (accessor == null)
        {
            Debug.LogError("Shared memory accessor is not initialized.");
            return;
        }

        try
        {
            accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading from shared memory: " + e.Message);
        }
    }
}
