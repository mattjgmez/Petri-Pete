using UnityEngine;
using System.Runtime.InteropServices; // For DLLImport
using System;
using System.IO.MemoryMappedFiles;

public class ExternalWindowRenderer : MonoBehaviour
{
    // DLL Import
    [DllImport("SecondaryWindowPlugin")]
    private static extern void CreateNewWindow(string windowName);

    [DllImport("SecondaryWindowPlugin")]
    private static extern void SendImageDataToSecondaryWindow(byte[] data, int length, int width, int height);

    private const string SHARED_MEMORY_NAME = "Local\\MySharedMemory";

    // Texture and Buffer
    public int DesiredWidth = 1920;  // Adjust as needed
    public int DesiredHeight = 1080; // Adjust as needed
    public string WindowTitle = "New Window";
    private Texture2D sharedMemoryTexture;
    private byte[] textureBuffer;

    public Material displayMaterial; // Assign in inspector

    void Start()
    {
        // Initialize texture and buffer
        sharedMemoryTexture = new Texture2D(DesiredWidth, DesiredHeight, TextureFormat.RGBA32, false);
        textureBuffer = new byte[DesiredWidth * DesiredHeight * 4];

        displayMaterial.mainTexture = sharedMemoryTexture;

        // Initialize shared memory
        InitializeSharedMemory();

        // Assuming you want to create a new window on start:
        try
        {
            CreateNewWindow(WindowTitle);
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
        SendImageDataToSecondaryWindow(imageData, imageData.Length, DesiredWidth, DesiredHeight);
    }

    void OnDestroy()
    {
        accessor?.Dispose();
        memoryMappedFile?.Dispose();
    }

    private MemoryMappedFile memoryMappedFile;
    private MemoryMappedViewAccessor accessor;

    private void InitializeSharedMemory()
    {
        try
        {
            memoryMappedFile = MemoryMappedFile.OpenExisting(SHARED_MEMORY_NAME);
            accessor = memoryMappedFile.CreateViewAccessor();
        }
        catch (Exception e)
        {
            Debug.LogError("Error initializing shared memory: " + e.Message);
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
