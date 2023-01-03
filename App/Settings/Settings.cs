using System.Text.Json;

namespace Khai;

public class Settings
{
    Theme theme;
    int width;
    int height;
    int tableWidth;
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

    public Settings()
    {
        Theme = new Theme();
        Width = 150;
        Height = 45;
        TableWidth= 110;
        TimeWidth = 23;
    }
}