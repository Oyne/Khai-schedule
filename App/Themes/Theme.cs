using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

namespace Khai;

/// <summary>
/// class for setting color theme
/// </summary>
public class Theme
{
    /// <summary>
    /// array of colors of a theme
    /// </summary>
    ConsoleColor[] colors;
    /// <summary>
    /// arary of all colors
    /// </summary>
    readonly ConsoleColor[] examples = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));
    /// <summary>
    /// schedule table dimensions
    /// </summary>
    readonly int tableWidth, timeWidth;

    public ConsoleColor[] Colors
    {
        get
        {
            return colors;
        }
        set
        {
            this.colors = value;
        }
    }

    /// <summary>
    /// constructor that creats default theme
    /// </summary>
    public Theme()
    {
        this.colors = new ConsoleColor[10];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = ConsoleColor.Black;
        }
        for (int i = 5; i < colors.Length; i++)
        {
            colors[i] = ConsoleColor.Gray;
        }
        colors[2] = ConsoleColor.DarkYellow;
        colors[7] = ConsoleColor.Black;
    }

    /// <summary>
    /// constructor that creates theme with colors from "colors" array
    /// </summary>
    /// <param name="colors">array of colors</param>
    public Theme(ConsoleColor[] colors)
    {
        this.colors = colors;
    }

    /// <summary>
    /// method for setting theme for console
    /// </summary>
    /// <param name="theme">array of colors</param>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    /// <returns></returns>
    public static int SetColor(ref Theme theme, int tableWidth, int timeWidth)
    {
        int ret = 0;
        bool boolean = true;
        ConsoleKeyInfo keyInfo;

    MenuCommand:
        Console.Clear();

        Console.Write("\n\n" + new string(' ', (Console.WindowWidth - "  Настройки темы  ".Length) / 2) + "|");
        Console.Write(" Настройки темы ");
        Console.WriteLine("|");
        Console.WriteLine('\n');

        Console.Write("""
            1. Изменить цвет фона <1>
            2. Изменить цвет текста <2>
            3. Изменить цвет фона знаменателя <3>
            4. Изменить цвет текста знаменателя <4>
            5. Установить тему по умолчанию <5>
            6. Сбросить все цвета <6>
            7. Вернуться в предыдущее меню <7>
            8. Вернуться в главное меню <8>
            9. Выход <Esc>
            >>> 
            """
        );

        while (boolean == true)
        {
            keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.D1:
                    {
                        theme.SetBackColor(tableWidth, timeWidth);
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D2:
                    {
                        theme.SetFrontColor(tableWidth, timeWidth);
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D3:
                    {
                        theme.SetDenBackColor(tableWidth, timeWidth);
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D4:
                    {
                        theme.SetDenTextColor(tableWidth, timeWidth);
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D5:
                    {
                        theme.SetDefaultTheme();
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D6:
                    {
                        theme.SetDefaultColors();
                        goto MenuCommand;
                    }
                    break;
                case ConsoleKey.D7:
                    {
                        ret = 2;
                        boolean = false;
                    }
                    break;
                case ConsoleKey.D8:
                    {                            
                        boolean = false;
                    }
                    break;
                case ConsoleKey.Tab:
                    {
                        Minimize.MinimizeConsoleWindow();
                    }
                    break;
                case ConsoleKey.Escape:
                    {
                        ret = 1;
                        boolean = false;
                    }
                    break;
            }
        }
        return ret;
    }

    /// <summary>
    /// outputing instruction how to choose colors
    /// </summary>
    public static void Instuction()
    {
        Console.Write(new string(' ', (Console.WindowWidth - "  ← → - вперёд/назад".Length) / 2));
        Console.WriteLine(" ← →  - вперёд/назад\n");
        Console.Write(new string(' ', (Console.WindowWidth - "Enter - выбор цвета".Length) / 2));
        Console.WriteLine("Enter - выбор цвета\n\n");
    }

    /// <summary>
    /// setting background color of console
    /// </summary>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    public void SetBackColor(int tableWidth, int timeWidth)
    {
        ConsoleColor currentForeground = Console.ForegroundColor;
        ConsoleColor currentBackground = Console.BackgroundColor;
        ConsoleColor DenBackColor = colors[2];
        ConsoleColor DenTextColor = colors[7];

        ConsoleKeyInfo keyInfo;
        bool enter = false;

        for (int i = 0; i <= 15; i++)
        {
            if (examples[i] == currentForeground || (DenBackColor == currentBackground && examples[i] == DenTextColor)) continue;
            Console.BackgroundColor = examples[i];
            Console.Clear();
            for (int j = 0; j < 5; j++)
            {
                if (currentBackground != DenBackColor && j == 2) continue;
                colors[j] = examples[i];
            }
            Console.WriteLine("\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - "Выберите цвет фона".Length) / 2) + "Выберите цвет фона\n\n");
            Instuction();
            PrintTable(colors, tableWidth, timeWidth);
            bool boolean = true;
            while (boolean == true && enter == false)
            {
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (i > 1 && (examples[i - 1] == currentForeground || examples[i-1] == DenTextColor)) i -= 3;
                            else if (i == 1 && (examples[i - 1] == currentForeground || examples[i-1] == DenTextColor)) i = 14;
                            else if (i == 0 && (examples[^1] == currentForeground || examples[^1] == DenTextColor)) i = 13;
                            else if (i > 0) i -= 2;
                            else i = 14;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        {
                            if (i == 15) i = -1;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            enter = true;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Escape:
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            if (enter) break;
        }
    }

    /// <summary>
    /// setting foreground color of console
    /// </summary>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    public void SetFrontColor(int tableWidth, int timeWidth)
    {
        ConsoleColor currentForeground = Console.ForegroundColor;
        ConsoleColor currentBackground = Console.BackgroundColor;
        ConsoleColor DenFrontColor = colors[7];
        ConsoleKeyInfo keyInfo;
        bool enter = false;

        for (int i = 0; i <= 15; i++)
        {
            if (examples[i] == currentBackground) continue;
            Console.ForegroundColor = examples[i];
            Console.Clear();
            for (int j = 5; j < colors.Length; j++)
            {
                if (currentForeground != DenFrontColor && j == 7) continue;
                colors[j] = examples[i];
            }
            Console.WriteLine("\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - "Выберите цвет текста".Length) / 2) + "Выберите цвет текста\n\n");
            Instuction();
            PrintTable(colors, tableWidth, timeWidth);
            bool boolean = true;
            while (boolean == true && enter == false)
            {
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (i > 1 && examples[i - 1] == currentBackground) i -= 3;
                            else if (i == 1 && examples[i - 1] == currentBackground) i = 14;
                            else if (i == 0 && examples[^1] == currentBackground) i = 13;
                            else if (i > 0) i -= 2;
                            else i = 14;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        {
                            if (i == 15) i = -1;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            enter = true;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Escape:
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            if (enter) break;
        }
    }

    /// <summary>
    /// setting denominator background color
    /// </summary>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    public void SetDenBackColor(int tableWidth, int timeWidth)
    {
        ConsoleColor currentDenTextColor = colors[7];
        ConsoleKeyInfo keyInfo;
        bool enter = false;

        for (int i = 0; i <= 15; i++)
        {
            if (examples[i] == currentDenTextColor) continue;
            Console.Clear();

            colors[2] = examples[i];

            Console.WriteLine("\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - "Выберите цвет фона знаменателя".Length) / 2) + "Выберите цвет фона знаменателя\n\n");
            Instuction();
            PrintTable(colors, tableWidth, timeWidth);
            bool boolean = true;
            while (boolean == true && enter == false)
            {
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (i > 1 && examples[i - 1] == currentDenTextColor) i -= 3;
                            else if (i == 1 && examples[i - 1] == currentDenTextColor) i = 14;
                            else if (i == 0 && examples[^1] == currentDenTextColor) i = 13;
                            else if (i > 0) i -= 2;
                            else i = 14;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        {
                            if (i == 15) i = -1;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            enter = true;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Escape:
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            if (enter) break;
        }
    }

    /// <summary>
    /// setting denominator foreground color
    /// </summary>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    public void SetDenTextColor(int tableWidth, int timeWidth)
    {
        ConsoleColor currentDenBackColor = colors[2];
        ConsoleKeyInfo keyInfo;
        bool enter = false;

        for (int i = 0; i <= 15; i++)
        {
            if (examples[i] == currentDenBackColor) continue;
            Console.Clear();

            colors[7] = examples[i];

            Console.WriteLine("\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - "Выберите цвет текста знаменателя".Length) / 2) + "Выберите цвет текста знаменателя\n\n");
            Instuction();
            PrintTable(colors, tableWidth, timeWidth);
            bool boolean = true;
            while (boolean == true && enter == false)
            {
                keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        {
                            if (i > 1 && examples[i - 1] == currentDenBackColor) i -= 3;
                            else if (i == 1 && examples[i - 1] == currentDenBackColor) i = 14;
                            else if (i == 0 && examples[^1] == currentDenBackColor) i = 13;
                            else if (i > 0) i -= 2;
                            else i = 14;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        {
                            if (i == 15) i = -1;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Enter:
                        {
                            enter = true;
                            boolean = false;
                        }
                        break;
                    case ConsoleKey.Escape:
                        {
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            if (enter) break;
        }
    }

    /// <summary>
    /// setting default theme
    /// </summary>
    public void SetDefaultTheme()
    {
        for (int i = 0; i < 5; i++)
        {
            colors[i] = ConsoleColor.Black;
        }
        colors[2] = ConsoleColor.DarkYellow;
        Console.BackgroundColor = colors[0];
        for (int i = 5; i < colors.Length; i++)
        {
            colors[i] = ConsoleColor.Gray;
        }
        colors[7] = ConsoleColor.Black;
        Console.ForegroundColor = colors[5];
    }

    /// <summary>
    /// setting all colors to default
    /// </summary>
    public void SetDefaultColors()
    {
        for (int i = 0; i < 5; i++)
        {
            colors[i] = ConsoleColor.Black;
        }
        Console.BackgroundColor = colors[0];
        for (int i = 5; i < colors.Length; i++)
        {
            colors[i] = ConsoleColor.Gray;
        }
        Console.ForegroundColor = colors[5];
    }

    /// <summary>
    /// method for printing schedule table
    /// </summary>
    /// <param name="colors">array of colors of a theme</param>
    /// <param name="tableWidth">table width value</param>
    /// <param name="timeWidth">time field width value</param>
    public static void PrintTable(ConsoleColor[] colors, int tableWidth, int timeWidth)
    {
        int TABLEWIDTH = tableWidth; // set general table size (default: 101)
        int TIMEWIDTH = timeWidth; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
        int SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3))

        string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };

        string output = "", out_num = "", out_den = "";
        int den = 0, num = 0, count = 0;

        ConsoleColor CurrentBackColor = colors[0];
        ConsoleColor TableBackColor = CurrentBackColor; // background color of the table
        ConsoleColor NumBackColor = colors[1];        // background color of the numerator
        ConsoleColor DenBackColor = colors[2];        // background color of the denominator
        ConsoleColor TimeBackColor = colors[3];        // background color of the time field
        ConsoleColor DayOfWeekBackColor = colors[4];        // background color of the day of week field
        ConsoleColor CurrentTextColor = colors[5];
        ConsoleColor TableTextColor = CurrentTextColor; // font color of the table         
        ConsoleColor NumTextColor = colors[6];        // font color of the numerator         
        ConsoleColor DenTextColor = colors[7];        // font color of the denominator       
        ConsoleColor TimeTextColor = colors[8];        // font color of the time field        
        ConsoleColor DayOfWeekTextColor = colors[9];        // font color of the day of week field 
        //ConsoleColor BlankBackColor = ConsoleColor.DarkBlue;
        //ConsoleColor BlankTextColor = ConsoleColor.White;

        SetColor(TableBackColor, TableTextColor);
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(DayOfWeekBackColor, DayOfWeekTextColor);
        Console.Write(new string(' ', ((TABLEWIDTH - 2) - "П'ятниця".Length) / 2) + "П'ятниця" + new string(' ', (TABLEWIDTH - 2) - ((TABLEWIDTH - 2) - "П'ятниця".Length) / 2 - "П'ятниця".Length));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[0].Length) / 2) + timeOfPairs[0] + new string(' ', TIMEWIDTH - timeOfPairs[0].Length - ((TIMEWIDTH - timeOfPairs[0].Length) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");

        output += "233р, Технології програмування, лекція";
        num += "233р, Технології програмування, лекція".Length;
        SetColor(NumBackColor, NumTextColor);
        if (num > SUBJECTWIDTH)
        {
            int temp = (num / SUBJECTWIDTH);
            int counter = 0;
            Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
            counter++;
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("|");
            while (temp != 1)
            {
                Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                SetColor(TableBackColor, TableTextColor);
                Console.Write(new string(' ', TIMEWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.Write("|");
                SetColor(NumBackColor, NumTextColor);
                Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.WriteLine("|");
                counter++;
                temp--;
            }
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TableBackColor, TableTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(output.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH * counter).Length));
        }
        else if (num == SUBJECTWIDTH) Console.Write(output);
        else Console.Write(new string(' ', (SUBJECTWIDTH - num) / 2) + output + new string(' ', SUBJECTWIDTH - num - ((SUBJECTWIDTH - num) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

        num = 0;
        den = 0;
        out_num += "136ар, Комп'ютерна схемотехніка, лаб. практикум";
        out_den += "210лк, Вища математика, лекція";
        num += "136ар, Комп'ютерна схемотехніка, лаб. практикум".Length;
        den += "210лк, Вища математика, лекція".Length;

        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        SetColor(NumBackColor, NumTextColor);
        if (num > SUBJECTWIDTH)
        {
            int temp = (num / SUBJECTWIDTH);
            int counter = 0;
            Console.Write(out_num.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
            counter++;
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("|");
            while (temp != 1)
            {
                Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                SetColor(TableBackColor, TableTextColor);
                Console.Write(new string(' ', TIMEWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.Write("|");
                SetColor(NumBackColor, NumTextColor);
                Console.Write(out_num.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.WriteLine("|");
                counter++;
                temp--;
            }
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TableBackColor, TableTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(out_num.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - out_num.Substring(SUBJECTWIDTH * counter).Length));
        }
        else if (num == SUBJECTWIDTH) Console.Write(out_num);
        else Console.Write(new string(' ', (SUBJECTWIDTH - num) / 2) + out_num + new string(' ', SUBJECTWIDTH - num - ((SUBJECTWIDTH - num) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[1].Length) / 2) + timeOfPairs[1] + new string(' ', TIMEWIDTH - timeOfPairs[1].Length - ((TIMEWIDTH - timeOfPairs[1].Length) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        Console.WriteLine(new string('-', SUBJECTWIDTH + 1));
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        SetColor(DenBackColor, DenTextColor);
        if (den > SUBJECTWIDTH)
        {
            int temp = (den / SUBJECTWIDTH);
            int counter = 0;
            Console.Write(out_den.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
            counter++;
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("|");
            while (temp != 1)
            {
                Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                SetColor(TableBackColor, TableTextColor);
                Console.Write(new string(' ', TIMEWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.Write("|");
                SetColor(NumBackColor, NumTextColor);
                Console.Write(out_den.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.WriteLine("|");
                counter++;
                temp--;
            }
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TableBackColor, TableTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(out_den.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - out_den.Substring(SUBJECTWIDTH * counter).Length));
        }
        else if (den == SUBJECTWIDTH) Console.Write(out_den);
        else Console.Write(new string(' ', (SUBJECTWIDTH - den) / 2) + out_den + new string(' ', SUBJECTWIDTH - den - ((SUBJECTWIDTH - den) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

        den = 0;
        output = "";
        output += "207аг, Філософія, практика";
        den += "207аг, Філософія, практика".Length;

        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        //Console.BackgroundColor = BlankBackColor;
        //Console.ForegroundColor = BlankTextColor;
        Console.Write(new string(' ', (SUBJECTWIDTH - 19) / 2) + "*******************" + new string(' ', SUBJECTWIDTH - 19 - (SUBJECTWIDTH - 19) / 2));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[2].Length) / 2) + timeOfPairs[2] + new string(' ', TIMEWIDTH - timeOfPairs[2].Length - ((TIMEWIDTH - timeOfPairs[2].Length) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        Console.WriteLine(new string('-', SUBJECTWIDTH + 1));
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        SetColor(DenBackColor, DenTextColor);
        if (den > SUBJECTWIDTH)
        {
            int temp = (den / SUBJECTWIDTH);
            int counter = 0;
            Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
            counter++;
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("|");
            while (temp != 1)
            {
                Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                SetColor(TableBackColor, TableTextColor);
                Console.Write(new string(' ', TIMEWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.Write("|");
                SetColor(NumBackColor, NumTextColor);
                Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.WriteLine("|");
                counter++;
                temp--;
            }
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TableBackColor, TableTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(output.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH * counter).Length));
        }
        else if (den == SUBJECTWIDTH) Console.Write(output);
        else Console.Write(new string(' ', (SUBJECTWIDTH - den) / 2) + output + new string(' ', SUBJECTWIDTH - den - ((SUBJECTWIDTH - den) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

        num = 0;
        output = "";
        output += "Фізичне виховання";
        num += "Фізичне виховання".Length;

        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        SetColor(NumBackColor, NumTextColor);
        if (num > SUBJECTWIDTH)
        {
            int temp = (num / SUBJECTWIDTH);
            int counter = 0;
            Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
            counter++;
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("|");
            while (temp != 1)
            {
                Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                SetColor(TableBackColor, TableTextColor);
                Console.Write(new string(' ', TIMEWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.Write("|");
                SetColor(NumBackColor, NumTextColor);
                Console.Write(output.Substring(SUBJECTWIDTH * counter, SUBJECTWIDTH));
                SetColor(TableBackColor, TableTextColor);
                Console.WriteLine("|");
                counter++;
                temp--;
            }
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TableBackColor, TableTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(output.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH * counter).Length));
        }
        else if (num == SUBJECTWIDTH) Console.Write(output);
        else Console.Write(new string(' ', (SUBJECTWIDTH - num) / 2) + output + new string(' ', SUBJECTWIDTH - num - ((SUBJECTWIDTH - num) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[3].Length) / 2) + timeOfPairs[3] + new string(' ', TIMEWIDTH - timeOfPairs[3].Length - ((TIMEWIDTH - timeOfPairs[3].Length) / 2)));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        Console.WriteLine(new string('-', SUBJECTWIDTH + 1));
        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
        SetColor(TimeBackColor, TimeTextColor);
        Console.Write(new string(' ', TIMEWIDTH));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|");
        //Console.BackgroundColor = BlankBackColor;
        //Console.ForegroundColor = BlankTextColor;
        SetColor(DenBackColor, DenTextColor);
        Console.Write(new string(' ', (SUBJECTWIDTH - 19) / 2) + "*******************" + new string(' ', SUBJECTWIDTH - 19 - (SUBJECTWIDTH - 19) / 2));
        SetColor(TableBackColor, TableTextColor);
        Console.Write("|\n");
        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
    }

    /// <summary>
    /// setting background and foreground colors
    /// </summary>
    /// <param name="back"> background color </param>
    /// <param name="front"> foreground color </param>
    public static void SetColor(ConsoleColor back, ConsoleColor front)
    {
        Console.BackgroundColor = back;
        Console.ForegroundColor = front;
    }
}