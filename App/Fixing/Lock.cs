using System.Runtime.InteropServices;

namespace Khai;

// Code to paste in main program
// Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MINIMIZE, Lock.MF_BYCOMMAND);
// Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MAXIMIZE, Lock.MF_BYCOMMAND);
// Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_SIZE, Lock.MF_BYCOMMAND);

public class Lock
{
    // code to fix size of a console
    /*====================================================================*/
    public const int MF_BYCOMMAND = 0x00000000;
    public const int SC_MINIMIZE = 0xF020;
    public const int SC_MAXIMIZE = 0xF030;
    public const int SC_SIZE = 0xF000;

    [DllImport("user32.dll")]
    public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [DllImport("user32.dll")]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("kernel32.dll", ExactSpelling = true)]
    public static extern IntPtr GetConsoleWindow();
    /*====================================================================*/
}