using System.Text.Json;
using Themes;

namespace Khai;

public class Settings
{
    Theme theme;

    public Theme Theme
    {
        get
        {
            return theme;
        }
        set
        {
            this.theme = value;
        }
    }

    public Settings()
    {
        theme = new Theme();
    }
}