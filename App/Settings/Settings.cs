using System.Text.Json;

namespace Khai;

public class Settings
{
    /// <summary>
    /// theme of console
    /// </summary>
    Theme theme;
    /// <summary>
    /// width of console
    /// </summary>
    int width;
    /// <summary>
    /// height of console
    /// </summary>
    int height;
    /// <summary>
    /// width of schedule table
    /// </summary>
    int tableWidth;
    /// <summary>
    /// heigth of schedule table
    /// </summary>
    int timeWidth;

    public Theme Theme
    {
        get {return theme;}
        set {this.theme = value;}
    }

    public int Width
    {
        get { return width; }
        set { this.width = value; }
    }

    public int Height
    {
        get {return height;}
        set {this.height = value;}
    }

    public int TableWidth
    {
        get { return tableWidth; }
        set { this.tableWidth = value; }
    }

    public int TimeWidth
    {
        get { return timeWidth; }
        set { this.timeWidth = value; }
    }

    /// <summary>
    /// constructor for setting default settings
    /// </summary>
    public Settings()
    {
        Theme = new Theme();
        Width = 150;
        Height = 45;
        TableWidth= 110;
        TimeWidth = 23;
    }
}