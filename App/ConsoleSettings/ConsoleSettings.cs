using System.Runtime.InteropServices;

namespace Khai;

public class ConsoleSettings
{
    // code to fix size of a console
    /*====================================================================*/
    const int MF_BYCOMMAND = 0x00000000;
    const int SC_MINIMIZE = 0xF020;
    const int SC_MAXIMIZE = 0xF030;
    const int SC_SIZE = 0xF000;
    
    [DllImport("user32.dll")]
    static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
    
    [DllImport("user32.dll")]
    static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    
    [DllImport("kernel32.dll", ExactSpelling = true)]
    static extern IntPtr GetConsoleWindow();
    /*====================================================================*/
    
    int width = 150; // width of a console
    int height = 45; // heught of a console
    Console.SetWindowSize(width, height); // set console size
    
    // code to fix size of a concole
    DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
    DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
    DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
    
    // code to minimize a console
    /*====================================================================*/
    const Int32 SW_MINIMIZE = 6;
    
    //[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    //static extern IntPtr GetConsoleWindow();
    
    [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);
    
    static void MinimizeConsoleWindow()
    {
        IntPtr hWndConsole = GetConsoleWindow();
        ShowWindow(hWndConsole, SW_MINIMIZE);
    }
    /*====================================================================*/
}
