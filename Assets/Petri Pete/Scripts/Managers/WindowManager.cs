using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    [DllImport("SecondaryWindowPlugin")]
    private static extern void CreateNewWindow(string windowName);

    protected virtual void Start()
    {
        CreateNewWindow("TestWindow");
    }
}
