using System.Runtime.InteropServices;
using UnityEngine;

public static class WindowsMessageBox
{
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int MessageBox(
        System.IntPtr hWnd,
        string text,
        string caption,
        uint type);

    private const uint MB_OK = 0x00000000;

    public static void Show(string message, string title = "Message")
    {
        MessageBox(System.IntPtr.Zero, message, title, MB_OK);
    }
#else
    public static void Show(string message, string title = "Message")
    {
        Debug.Log($"{title}: {message}");
    }
#endif
}
