using System.Runtime.InteropServices;

namespace Khai;

/// <summary>
/// class for minimizing console
/// </summary>
public class Minimize
{
    const Int32 SW_MINIMIZE = 6;

    [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    static extern IntPtr GetConsoleWindow();

    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);

    public static void MinimizeConsoleWindow()
    {
        IntPtr hWndConsole = GetConsoleWindow();
        ShowWindow(hWndConsole, SW_MINIMIZE);
    }
}