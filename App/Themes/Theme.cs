using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using Minimizing;

namespace Themes
{
    public class Theme
    {
        ConsoleColor[] colors;
        ConsoleColor[] examples = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

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

        public Theme(ConsoleColor[] colors)
        {
            this.colors = colors;
        }

        public static int SetColor(ref Theme theme)
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
                            theme.SetBackColor();
                            goto MenuCommand;
                        }
                        break;
                    case ConsoleKey.D2:
                        {
                            theme.SetFrontColor();
                            goto MenuCommand;
                        }
                        break;
                    case ConsoleKey.D3:
                        {
                            theme.SetDenBackColor();
                            goto MenuCommand;
                        }
                        break;
                    case ConsoleKey.D4:
                        {
                            theme.SetDenTextColor();
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
        public static void Instuction()
        {
            Console.Write(new string(' ', (Console.WindowWidth - "  ← → - вперёд/назад".Length) / 2));
            Console.WriteLine(" ← →  - вперёд/назад");
            Console.Write(new string(' ', (Console.WindowWidth - "Enter - выбор цвета".Length) / 2));
            Console.WriteLine("Enter - выбор цвета\n\n");
        }

        public void SetBackColor()
        {
            ConsoleColor currentForeground = Console.ForegroundColor;
            ConsoleColor currentBackground = Console.BackgroundColor;
            ConsoleColor DenBackColor = colors[2];

            ConsoleKeyInfo keyInfo;
            bool enter = false;

            for (int i = 0; i <= 15; i++)
            {
                if (examples[i] == currentForeground) continue;
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
                PrintTable(colors);
                bool boolean = true;
                while (boolean == true && enter == false)
                {
                    keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            {
                                if (i > 1 && examples[i - 1] == currentForeground) i -= 3;
                                else if (i == 1 && examples[i - 1] == currentForeground) i = 14;
                                else if (i == 0 && examples[^1] == currentForeground) i = 13;
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

        public void SetFrontColor()
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
                PrintTable(colors);
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

        public void SetDenBackColor()
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
                PrintTable(colors);
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

        public void SetDenTextColor()
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
                PrintTable(colors);
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

        public static void PrintTable(ConsoleColor[] colors)
        {
            const int TABLEWIDTH = 110; // set general table size (default: 101)
            const int TIMEWIDTH = 23; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
            const int SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3))

            string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };

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
            Console.Write(new string(' ', (SUBJECTWIDTH - "233р, Технології програмування, лекція".Length) / 2) + "233р, Технології програмування, лекція" + new string(' ', SUBJECTWIDTH - "233р, Технології програмування, лекція".Length - ((SUBJECTWIDTH - "233р, Технології програмування, лекція".Length) / 2)));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TimeBackColor, TimeTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(new string(' ', (SUBJECTWIDTH - "136ар, Комп'ютерна схемотехніка, лаб. практикум".Length) / 2) + "136ар, Комп'ютерна схемотехніка, лаб. практикум" + new string(' ', SUBJECTWIDTH - "136ар, Комп'ютерна схемотехніка, лаб. практикум".Length - ((SUBJECTWIDTH - "136ар, Комп'ютерна схемотехніка, лаб. практикум".Length) / 2)));
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
            Console.Write(new string(' ', (SUBJECTWIDTH - "210лк, Вища математика, лекція".Length) / 2) + "210лк, Вища математика, лекція" + new string(' ', SUBJECTWIDTH - "210лк, Вища математика, лекція".Length - ((SUBJECTWIDTH - "210лк, Вища математика, лекція".Length) / 2)));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TimeBackColor, TimeTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(new string(' ', (SUBJECTWIDTH - 19) / 2) + "*******************" + new string(' ', SUBJECTWIDTH - 19 - (SUBJECTWIDTH - 19) / 2));
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
            Console.Write(new string(' ', (SUBJECTWIDTH - "207аг, Філософія, практика".Length) / 2) + "207аг, Філософія, практика" + new string(' ', SUBJECTWIDTH - "207аг, Філософія, практика".Length - ((SUBJECTWIDTH - "207аг, Філософія, практика".Length) / 2)));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));

            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(TimeBackColor, TimeTextColor);
            Console.Write(new string(' ', TIMEWIDTH));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|");
            SetColor(NumBackColor, NumTextColor);
            Console.Write(new string(' ', (SUBJECTWIDTH - "Фізичне виховання".Length) / 2) + "Фізичне виховання" + new string(' ', SUBJECTWIDTH - "Фізичне виховання".Length - ((SUBJECTWIDTH - "Фізичне виховання".Length) / 2)));
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
            Console.Write(new string(' ', (SUBJECTWIDTH - 19) / 2) + "*******************" + new string(' ', SUBJECTWIDTH - 19 - (SUBJECTWIDTH - 19) / 2));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
        }
        public static void SetColor(ConsoleColor back, ConsoleColor front)
        {
            Console.BackgroundColor = back;
            Console.ForegroundColor = front;
        }
    }
}