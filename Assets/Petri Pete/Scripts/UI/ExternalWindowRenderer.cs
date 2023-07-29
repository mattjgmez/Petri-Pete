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

    // Initializing Shared Memory retry constants:
    private const int RETRY_COUNT = 3;
    private const int RETRY_DELAY = 1000; // milliseconds


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
            Debug.LogError("Failed to initialize shared memory. Disabling ExternalWindowRenderer.", gameObject);
            this.enabled = false; // Disable this component
            return;
        }

        // Assuming you want to create a new window on start:
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
        // Update texture with shared memory data
        ReadSharedMemoryIntoBuffer(textureBuffer);

        sharedMemoryTexture.LoadRawTextureData(textureBuffer);
        sharedMemoryTexture.Apply();

        byte[] imageData = sharedMemoryTexture.GetRawTextureData();

        if (imageData != null && imageData.Length == DesiredWidth * DesiredHeight * 4)
        {
            Debug.Log("Image data is being sent to Secondary Window.", gameObject);
            SendImageDataToSecondaryWindow(imageData, imageData.Length, DesiredWidth, DesiredHeight);
        }
        else 
        {
            if (imageData == null) 
            {
                Debug.LogError("Image data is null.", gameObject);
            }

            if (imageData.Length == DesiredWidth * DesiredHeight * 4) 
            {
                Debug.LogError("Image length is not as expected.", gameObject);
            }
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

    private void ReadSharedMemoryIntoBuffer(byte[] buffer)
    {
        if (accessor == null)
        {
            Debug.LogError("Shared memory accessor is not initialized.", gameObject);
            return;
        }

        try
        {
            accessor.ReadArray<byte>(0, buffer, 0, buffer.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Error reading from shared memory: " + e.Message, gameObject);
        }
    }
}
