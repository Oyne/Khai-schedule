using System.Runtime.InteropServices;

namespace Khai;

public class Maximize
{
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    private static IntPtr ThisConsole = GetConsoleWindow();
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int HIDE = 0;
    private const int MAXIMIZE = 3;
    private const int MINIMIZE = 6;
    private const int RESTORE = 9;

    public static IntPtr Ret1()
    {
        return ThisConsole;
    }
    public static int Ret2()
    {
        return MAXIMIZE;
    }
}
