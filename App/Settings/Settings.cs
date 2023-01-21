using System.Text.Json;

namespace Khai;

public class Settings
{
    public Theme Theme { get; set; } = new Theme();
    public int Width { get; set; } = 150;
    public int Height { get; set; } = 45;
    public int TableWidth { get; set; } = 110;
    public int TimeWidth { get; set; } = 23;
}