using Khai;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

//Console title
Console.Title = "Khai schedule";

//Check the udates
Update.CheckUpdate();

// settings deserialization
string settingsFilePath = "C://Khai/settings.json";
var options = new JsonSerializerOptions { WriteIndented = true };
Settings settings = null;
if (File.Exists(settingsFilePath))
{
    settings = FileWork.ReadSettingsFromFile();
}
else
{
    settings = new Settings();
}

// setting size of console
if (settings.Width > 211 && settings.Height > 49)
{
    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
    Maximize.ShowWindow(Maximize.Ret1(), Maximize.Ret2());
}
else
{
    int width = settings.Width; // width of console
    int height = settings.Height; // height of console
    Console.SetWindowSize(width, height); // set console size
}

// code to fix size of a concole
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MINIMIZE, Lock.MF_BYCOMMAND);
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MAXIMIZE, Lock.MF_BYCOMMAND);
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_SIZE, Lock.MF_BYCOMMAND);

// initializing color theme of console
Theme theme = settings.Theme;

while (true)
{
    // setting the correct ukrainian language output
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

    using var client = new KhaiClient();
    string group = "";
    string name = "";
    string exit = "";
    int choice, menu_item;
    bool boolean;
    string[] menu = {"› Поиск по группе ",
                     "› Поиск по студенту ",
                     "› Поиск по преподавателю ",
                     "› Считать расписание из файла ",
                     "› Настройки ",
                     "› Выход "};
    string[] settings_menu = {"› Настройки темы ",
                     "› Настройки размера консоли ",
                     "› Настройки размера таблицы расписания ",
                     "› Вернуться в главное меню ",
                     "› Выход "};
    string[] console_size_menu = {"› Настройка размера консоли вручную ",
                     "› Открывать консоль во весь экран по умолчанию ",
                     "› Вернуться в предыдущее меню ",
                     "› Вернуться в главное меню ",
                     "› Выход "};
    string[] before_file_menu = {new string(' ',(Console.WindowWidth - "› Сохранить расписание в текстовый файл".Length)/2) +
                     "› Сохранить расписание в текстовый файл ",
                     new string(' ',(Console.WindowWidth - "› Сохранить расписание в текстовый файл".Length)/2) +
                     "› Вернуться в главное меню ",
                     new string(' ',(Console.WindowWidth - "› Сохранить расписание в текстовый файл".Length)/2) +
                     "› Выход "};
    string[] after_file_menu = {new string(' ',(Console.WindowWidth - "› Вернуться в главное меню".Length)/2) +
                     "› Вернуться в главное меню ",
                     new string(' ',(Console.WindowWidth - "› Вернуться в главное меню".Length)/2) +
                     "› Выход "};
    string[] no_file_menu = {
                     "› Вернуться в главное меню ",
                     "› Выход "};


MenuCommand:

    Console.CursorVisible = false;
    WeekSchedule Schedule = null;
    boolean = true;
    menu_item = 0;
    ConsoleColor ConsBackColor = theme.Colors[0];
    ConsoleColor ConsTextColor = theme.Colors[5];
    Console.BackgroundColor = ConsBackColor;
    Console.ForegroundColor = ConsTextColor;
    Console.Clear();
    Output.PrintKhai();

    ConsoleKeyInfo keyInfo;

    while (boolean)
    {
        do
        {
            Console.SetCursorPosition(0, 5);
            Output.PrintMenu(menu, menu_item, theme);

            keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    {
                        if (menu_item == 0)
                        {
                            menu_item = menu.Length - 1;
                        }
                        else menu_item--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    {
                        if (menu_item == menu.Length - 1)
                        {
                            menu_item = 0;
                        }
                        else menu_item++;
                    }
                    break;
                case ConsoleKey.Tab:
                    {
                        Minimize.MinimizeConsoleWindow();
                    }
                    break;
                case ConsoleKey.Escape:
                    {
                        FileWork.SaveSettings(settings);
                        Environment.Exit(0);
                    }
                    break;
            }

        } while (keyInfo.Key != ConsoleKey.Enter);

        switch (menu_item)
        {
            case 0:
                {
                    while (Schedule == null)
                    {
                        Console.CursorVisible = true;
                        Console.Write("Введите группу в формате 325, 525v (525в), 116i1, 430st (430ст), 555vm-2 (555вм/2)\n>>> ");
                        group = Console.ReadLine();
                        if (group == "exit") Environment.Exit(0);
                        if (group == "back") goto MenuCommand;
                        try
                        {
                            Schedule = await client.GetGroupWeekSheduleAsync(group);
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Write("Некорректный ввод или группы не существует\n");
                        }
                        catch (Exception e)
                        {
                            Console.Write("Очень некорректный ввод, группы не существует или плохое интернет соединение\n");
                        }
                    }
                    Console.CursorVisible = false;
                    Console.Clear();
                    Output.PrintKhai();
                    Console.WriteLine("\n\n\n\n\n\n");
                    Console.WriteLine(new string(' ', (Console.WindowWidth - 8 - group.Length) / 2) + $"Группа: {group}");
                    await Task.Run(() => Output.Outputing(Schedule, theme.Colors, settings.TableWidth, settings.TimeWidth));
                    Console.SetCursorPosition(0, 0);
                    boolean = false;
                }
                break;
            case 1:
                {
                    while (Schedule == null)
                    {
                        Console.CursorVisible = true;
                        Console.Write("Введите имя в формате bondarenko-a-o, kuzmichov-i-i\n>>> ");
                        name = Console.ReadLine();
                        if (name == "exit") Environment.Exit(0);
                        if (name == "back") goto MenuCommand;
                        try
                        {
                            Schedule = await client.GetStudentWeekSheduleAsync(name);
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Write("Некорректный ввод или студента не существует\n");
                        }
                        catch (System.Net.Http.HttpRequestException e)
                        {
                            Console.Write("Некорректный ввод или студента не существует\n");
                        }
                        catch (Exception e)
                        {
                            Console.Write("Очень некорректный ввод, студента не существует или плохое интернет соединение\n");
                        }
                    }
                    Console.CursorVisible = false;
                    Console.Clear();
                    Output.PrintKhai();
                    Console.WriteLine("\n\n\n\n\n\n");
                    Console.WriteLine(new string(' ', (Console.WindowWidth - 9 - name.Length) / 2) + $"Студент: {name}");
                    await Task.Run(() => Output.Outputing(Schedule, theme.Colors, settings.TableWidth, settings.TimeWidth));
                    Console.SetCursorPosition(0, 0);
                    boolean = false;
                }
                break;
            case 2:
                {
                    while (Schedule == null)
                    {
                        Console.CursorVisible = true;
                        Console.Write("Введите имя в формате babeschko-e-v-503, savchenko-n-v-405\n>>> ");
                        name = Console.ReadLine();
                        if (name == "exit") Environment.Exit(0);
                        if (name == "back") goto MenuCommand;
                        try
                        {
                            Schedule = await client.GetLecturerWeekSheduleAsync(name);
                        }
                        catch (NullReferenceException e)
                        {
                            Console.Write("Некорректный ввод или преподавателя не существует\n");
                        }
                        catch (System.Net.Http.HttpRequestException e)
                        {
                            Console.Write("Некорректный ввод или преподавателя не существует\n");
                        }
                        catch (Exception e)
                        {
                            Console.Write("Очень некорректный ввод, преподавателя не существует или плохое интернет соединение\n");
                        }
                    }
                    Console.CursorVisible = false;
                    Console.Clear();
                    Output.PrintKhai();
                    Console.WriteLine("\n\n\n\n\n\n");
                    Console.WriteLine(new string(' ', (Console.WindowWidth - 15 - name.Length) / 2) + $"Преподаватель: {name}");
                    await Task.Run(() => Output.Outputing(Schedule, theme.Colors, settings.TableWidth, settings.TimeWidth));
                    Console.SetCursorPosition(0, 0);
                    boolean = false;
                }
                break;
            case 3:
                {
                    int file = 0;
                    try
                    {
                        Schedule = FileWork.ReadScheduleFromFile();

                        Console.BackgroundColor = ConsBackColor;
                        Console.ForegroundColor = ConsTextColor;
                        Console.Clear();
                        Output.PrintKhai();
                        Console.WriteLine("\n\n\n");
                        await Task.Run(() => Output.Outputing(Schedule, theme.Colors, settings.TableWidth, settings.TimeWidth));
                        Console.SetCursorPosition(0, 0);
                        boolean = false;
                    }
                    catch (FileWasNotFoundException e)
                    {
                        Console.WriteLine("Не удалось прочитать файл.\n");
                        file = 1;
                    }
                    catch (ScheduleWasNotFoundException e)
                    {
                        Console.WriteLine("Нет сохранённого расписания.\n");
                        file = 1;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Произошло что-то очень плохое\n");
                        file = 1;
                    }
                    menu_item = 0;
                    do
                    {
                        string[] Menu;
                        if (file == 1)
                        {
                            Menu = no_file_menu;
                            Console.SetCursorPosition(0, 13);
                        }
                        else
                        {
                            Menu = after_file_menu;
                            Console.SetCursorPosition(0, 5);
                        }
                        Output.PrintMenu(Menu, menu_item, theme);

                        keyInfo = Console.ReadKey();

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.UpArrow:
                                {
                                    if (menu_item == 0)
                                    {
                                        menu_item = Menu.Length - 1;
                                    }
                                    else menu_item--;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                {
                                    if (menu_item == Menu.Length - 1)
                                    {
                                        menu_item = 0;
                                    }
                                    else menu_item++;
                                }
                                break;
                            case ConsoleKey.Tab:
                                {
                                    Minimize.MinimizeConsoleWindow();
                                }
                                break;
                            case ConsoleKey.Escape:
                                {
                                    FileWork.SaveSettings(settings);
                                    Environment.Exit(0);
                                }
                                break;
                        }

                    } while (keyInfo.Key != ConsoleKey.Enter);

                    while (true)
                    {
                        switch (menu_item)
                        {
                            case 0:
                                {
                                    goto MenuCommand;
                                }
                                break;
                            case 1:
                                {
                                    FileWork.SaveSettings(settings);
                                    Environment.Exit(0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 4:
                {
                SettingsMenu:
                    menu_item = 0;
                    Console.Clear();
                    Console.Write("\n\n" + new string(' ', (Console.WindowWidth - "  Настройки  ".Length) / 2) + "|");
                    Console.Write(" Настройки ");
                    Console.WriteLine("|");
                    Console.WriteLine('\n');
                    do
                    {
                        Console.SetCursorPosition(0, 5);
                        Output.PrintMenu(settings_menu, menu_item, theme);

                        keyInfo = Console.ReadKey();

                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.UpArrow:
                                {
                                    if (menu_item == 0)
                                    {
                                        menu_item = settings_menu.Length - 1;
                                    }
                                    else menu_item--;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                {
                                    if (menu_item == settings_menu.Length - 1)
                                    {
                                        menu_item = 0;
                                    }
                                    else menu_item++;
                                }
                                break;
                            case ConsoleKey.Tab:
                                {
                                    Minimize.MinimizeConsoleWindow();
                                }
                                break;
                            case ConsoleKey.Escape:
                                {
                                    FileWork.SaveSettings(settings);
                                    Environment.Exit(0);
                                }
                                break;
                        }

                    } while (keyInfo.Key != ConsoleKey.Enter);

                    while (true)
                    {
                        switch (menu_item)
                        {
                            case 0:
                                {
                                    Console.Clear();
                                    int ret = Theme.SetColor(ref theme, settings.TableWidth, settings.TimeWidth);
                                    if (ret == 1)
                                    {
                                        FileWork.SaveSettings(settings);
                                        Environment.Exit(0);
                                    }
                                    else if (ret == 2)
                                    {
                                        FileWork.SaveSettings(settings);
                                        goto SettingsMenu;
                                    }
                                    else
                                    {
                                        FileWork.SaveSettings(settings);
                                        goto MenuCommand;
                                    }
                                }
                                break;
                            case 1:
                                {
                                    menu_item = 0;
                                SizeMenu:
                                    Console.Clear();
                                    Console.Write("\n\n" + new string(' ', (Console.WindowWidth - "  Настройки размера консоли  ".Length) / 2) + "|");
                                    Console.Write(" Настройки размера консоли ");
                                    Console.WriteLine("|");
                                    Console.WriteLine('\n');
                                    do
                                    {
                                        Console.SetCursorPosition(0, 5);
                                        Output.PrintMenu(console_size_menu, menu_item, theme);

                                        keyInfo = Console.ReadKey();

                                        switch (keyInfo.Key)
                                        {
                                            case ConsoleKey.UpArrow:
                                                {
                                                    if (menu_item == 0)
                                                    {
                                                        menu_item = console_size_menu.Length - 1;
                                                    }
                                                    else menu_item--;
                                                }
                                                break;
                                            case ConsoleKey.DownArrow:
                                                {
                                                    if (menu_item == console_size_menu.Length - 1)
                                                    {
                                                        menu_item = 0;
                                                    }
                                                    else menu_item++;
                                                }
                                                break;
                                            case ConsoleKey.Tab:
                                                {
                                                    Minimize.MinimizeConsoleWindow();
                                                }
                                                break;
                                            case ConsoleKey.Escape:
                                                {
                                                    FileWork.SaveSettings(settings);
                                                    Environment.Exit(0);
                                                }
                                                break;
                                        }

                                    } while (keyInfo.Key != ConsoleKey.Enter);

                                    while (true)
                                    {
                                        switch (menu_item)
                                        {
                                            case 0:
                                                {
                                                    int[] arr = Khai.Size.SetSize(theme.Colors, settings.TableWidth, settings.TimeWidth);
                                                    settings.Width = arr[0];
                                                    settings.Height = arr[1];
                                                    FileWork.SaveSettings(settings);
                                                    goto SizeMenu;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
                                                    Maximize.ShowWindow(Maximize.Ret1(), Maximize.Ret2());
                                                    settings.Width = 212;
                                                    settings.Height = 50;
                                                    FileWork.SaveSettings(settings);
                                                    goto SizeMenu;
                                                }
                                                break;
                                            case 2:
                                                {
                                                    goto SettingsMenu;
                                                }
                                                break;
                                            case 3:
                                                {
                                                    goto MenuCommand;
                                                }
                                                break;
                                            case 4:
                                                {
                                                    FileWork.SaveSettings(settings);
                                                    Environment.Exit(0);
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            case 2:
                                {
                                    int[] arr = TableSize.SetTableSize(theme.Colors, settings.TableWidth, settings.TimeWidth);
                                    settings.TableWidth = arr[0];
                                    settings.TimeWidth = arr[1];
                                    FileWork.SaveSettings(settings);
                                    goto SettingsMenu;
                                }
                                break;
                            case 3:
                                {
                                    goto MenuCommand;
                                }
                                break;
                            case 4:
                                {
                                    FileWork.SaveSettings(settings);
                                    Environment.Exit(0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 5:
                {
                    FileWork.SaveSettings(settings);
                    Environment.Exit(0);
                }
                break;
        }
    }

    while (true)
    {
        menu_item = 0;
        do
        {
            Console.SetCursorPosition(0, 6);
            Output.PrintMenu(before_file_menu, menu_item, theme);

            keyInfo = Console.ReadKey();

            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    {
                        if (menu_item == 0)
                        {
                            menu_item = before_file_menu.Length - 1;
                        }
                        else menu_item--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    {
                        if (menu_item == before_file_menu.Length - 1)
                        {
                            menu_item = 0;
                        }
                        else menu_item++;
                    }
                    break;
                case ConsoleKey.Tab:
                    {
                        Minimize.MinimizeConsoleWindow();
                    }
                    break;
                case ConsoleKey.Escape:
                    {
                        FileWork.SaveSettings(settings);
                        Environment.Exit(0);
                    }
                    break;
            }

        } while (keyInfo.Key != ConsoleKey.Enter);
        int tmp = 0;
        switch (menu_item)
        {
            case 0:
                {
                    try
                    {
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (DirectoryDoesNotExistException e)
                    {
                        Console.SetCursorPosition(0, 5);
                        tmp = 1;
                        Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("Путь для сохранения файла не найден   ");
                        FileWork.CreateAFile("C://Khai/info.json");
                        Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("Путь для сохранения файла создан \"C://Khai/\"   ");
                        Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("Файл с расписанием создан");
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (FileDoesNotExistException e)
                    {
                        Console.SetCursorPosition(0, 5);
                        tmp = 1;
                        Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("Файл для сохранения не найден   ");
                        FileWork.CreateAFile("C://Khai/info.json");
                        Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("Файл с расписанием создан по пути \"C://Khai/\"   ");
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (Exception e)
                    {
                        Console.Clear();
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                        Console.WriteLine("\nПроизошло что-то очень плохое");
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }
                    if (tmp == 0) Console.SetCursorPosition(0, 5);
                    else Console.SetCursorPosition(0, 7);
                    Console.Write(new string(' ', (Console.WindowWidth - "Файл для сохранения не найден   ".Length) / 2));
                    Console.WriteLine("Расписание успешно сохранено      ");
                    Console.WriteLine(new string(' ', Console.WindowWidth * 3));


                    boolean = true;
                    while (boolean)
                    {
                        menu_item = 0;
                        do
                        {
                            if (tmp == 0) Console.SetCursorPosition(0, 7);
                            else Console.SetCursorPosition(0, 9);
                            Output.PrintMenu(after_file_menu, menu_item, theme);

                            keyInfo = Console.ReadKey();

                            switch (keyInfo.Key)
                            {
                                case ConsoleKey.UpArrow:
                                    {
                                        if (menu_item == 0)
                                        {
                                            menu_item = after_file_menu.Length - 1;
                                        }
                                        else menu_item--;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    {
                                        if (menu_item == after_file_menu.Length - 1)
                                        {
                                            menu_item = 0;
                                        }
                                        else menu_item++;
                                    }
                                    break;
                                case ConsoleKey.Tab:
                                    {
                                        Minimize.MinimizeConsoleWindow();
                                    }
                                    break;
                                case ConsoleKey.Escape:
                                    {
                                        FileWork.SaveSettings(settings);
                                        Environment.Exit(0);
                                    }
                                    break;
                            }

                        } while (keyInfo.Key != ConsoleKey.Enter);

                        switch (menu_item)
                        {
                            case 0:
                                {
                                    Console.Clear();
                                    goto MenuCommand;
                                }
                                break;
                            case 1:
                                {
                                    FileWork.SaveSettings(settings);
                                    Environment.Exit(0);
                                }
                                break;
                        }
                    }
                }
                break;
            case 1:
                {
                    Console.Clear();
                    goto MenuCommand;
                }
                break;
            case 2:
                {
                    FileWork.SaveSettings(settings);
                    Environment.Exit(0);
                }
                break;
        }
    }


    Debugger.Break();

}

/// <summary>
/// class for outputing the schedule
/// </summary>
class Output
{
    //int const TABLEWIDTH = 110; // set general table size (default: 101)
    //int const TIMEWIDTH = 23; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
    //int const SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3))


    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };

    /// <summary>
    /// method to print the schedule
    /// </summary>
    /// <param name="Schedule"> parsed variable with schedule data </param>
    /// <returns> print schedule in console </returns>
    public async static Task Outputing(WeekSchedule Schedule, ConsoleColor[] colors, int tableWidth, int timeWidth)
    {
        int TABLEWIDTH = tableWidth; // set general table size (default: 101)
        int TIMEWIDTH = timeWidth; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
        int SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3)) (!!!!  NOT LESS THAN 19  !!!!))

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

        int count = 0;
        int den;
        int num;
        string output;

        foreach (var day in Schedule.AsDays())
        {
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
            SetColor(DayOfWeekBackColor, DayOfWeekTextColor);
            Console.Write(new string(' ', ((TABLEWIDTH - 2) - daysOfWeek[count].Length) / 2) + daysOfWeek[count] + new string(' ', (TABLEWIDTH - 2) - ((TABLEWIDTH - 2) - daysOfWeek[count].Length) / 2 - daysOfWeek[count].Length));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
            for (int j = 0; j < 5; j++)
            {
                if (j >= day.Classes.Count) break;
                output = "";
                num = 0;
                den = 0;
                if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator != null)
                {
                    Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                    SetColor(TimeBackColor, TimeTextColor);
                    Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) / 2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|");
                    if (day.Classes[j].Numerator.RoomNumber != null)
                    {
                        output += day.Classes[j].Numerator.RoomNumber + ", ";
                        num += day.Classes[j].Numerator.RoomNumber.Length + 2;
                    }
                    if (day.Classes[j].Numerator.Name != null)
                    {
                        output += day.Classes[j].Numerator.Name;
                        num += day.Classes[j].Numerator.Name.Length;
                    }
                    if (day.Classes[j].Numerator.Type != null)
                    {
                        output += ", " + day.Classes[j].Numerator.Type;
                        num += day.Classes[j].Numerator.Type.Length + 2;
                    }
                    if (day.Classes[j].Numerator.Teacher != null)
                    {
                        output += ", " + day.Classes[j].Numerator.Teacher;
                        num += day.Classes[j].Numerator.Teacher.Length + 2;
                    }
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
                }
                else if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator == null)
                {
                    Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                    SetColor(TimeBackColor, TimeTextColor);
                    Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) / 2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|");
                    //Console.BackgroundColor = BlankBackColor;
                    //Console.ForegroundColor = BlankTextColor;
                    Console.Write(new string(' ', (SUBJECTWIDTH - 19) / 2) + "*******************" + new string(' ', SUBJECTWIDTH - 19 - (SUBJECTWIDTH - 19) / 2));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|\n");
                    Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
                }
                else
                {
                    if (day.Classes[j].Numerator == null)
                    {
                        if (day.Classes[j].Denominator.RoomNumber != null)
                        {
                            output += day.Classes[j].Denominator.RoomNumber + ", ";
                            den += day.Classes[j].Denominator.RoomNumber.Length + 2;
                        }
                        if (day.Classes[j].Denominator.Name != null)
                        {
                            output += day.Classes[j].Denominator.Name;
                            den += day.Classes[j].Denominator.Name.Length;
                        }
                        if (day.Classes[j].Denominator.Type != null)
                        {
                            output += ", " + day.Classes[j].Denominator.Type;
                            den += day.Classes[j].Denominator.Type.Length + 2;
                        }
                        if (day.Classes[j].Denominator.Teacher != null)
                        {
                            output += ", " + day.Classes[j].Denominator.Teacher;
                            den += day.Classes[j].Denominator.Teacher.Length + 2;
                        }
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
                        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) / 2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
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
                                SetColor(DenBackColor, DenTextColor);
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
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(output.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH * counter).Length));
                        }
                        else if (den == SUBJECTWIDTH) Console.Write(output);
                        else Console.Write(new string(' ', (SUBJECTWIDTH - den) / 2) + output + new string(' ', SUBJECTWIDTH - den - ((SUBJECTWIDTH - den) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
                    }
                    else if (day.Classes[j].Denominator == null)
                    {
                        if (day.Classes[j].Numerator.RoomNumber != null)
                        {
                            output += day.Classes[j].Numerator.RoomNumber + ", ";
                            num += day.Classes[j].Numerator.RoomNumber.Length + 2;
                        }
                        if (day.Classes[j].Numerator.Name != null)
                        {
                            output += day.Classes[j].Numerator.Name;
                            num += day.Classes[j].Numerator.Name.Length;
                        }
                        if (day.Classes[j].Numerator.Type != null)
                        {
                            output += ", " + day.Classes[j].Numerator.Type;
                            num += day.Classes[j].Numerator.Type.Length + 2;
                        }
                        if (day.Classes[j].Numerator.Teacher != null)
                        {
                            output += ", " + day.Classes[j].Numerator.Teacher;
                            num += day.Classes[j].Numerator.Teacher.Length + 2;
                        }
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
                        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) / 2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
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
                    else
                    {
                        string out_num = "", out_den = "";
                        if (day.Classes[j].Numerator.RoomNumber != null)
                        {
                            out_num += day.Classes[j].Numerator.RoomNumber + ", ";
                            num += day.Classes[j].Numerator.RoomNumber.Length + 2;
                        }
                        if (day.Classes[j].Numerator.Name != null)
                        {
                            out_num += day.Classes[j].Numerator.Name;
                            num += day.Classes[j].Numerator.Name.Length;
                        }
                        if (day.Classes[j].Numerator.Type != null)
                        {
                            out_num += ", " + day.Classes[j].Numerator.Type;
                            num += day.Classes[j].Numerator.Type.Length + 2;
                        }
                        if (day.Classes[j].Numerator.Teacher != null)
                        {
                            out_num += ", " + day.Classes[j].Numerator.Teacher;
                            num += day.Classes[j].Numerator.Teacher.Length + 2;
                        }
                        if (day.Classes[j].Denominator.RoomNumber != null)
                        {
                            out_den += day.Classes[j].Denominator.RoomNumber + ", ";
                            den += day.Classes[j].Denominator.RoomNumber.Length + 2;
                        }
                        if (day.Classes[j].Denominator.Name != null)
                        {
                            out_den += day.Classes[j].Denominator.Name;
                            den += day.Classes[j].Denominator.Name.Length;
                        }
                        if (day.Classes[j].Denominator.Type != null)
                        {
                            out_den += ", " + day.Classes[j].Denominator.Type;
                            den += day.Classes[j].Denominator.Type.Length + 2;
                        }
                        if (day.Classes[j].Denominator.Teacher != null)
                        {
                            out_den += ", " + day.Classes[j].Denominator.Teacher;
                            den += day.Classes[j].Denominator.Teacher.Length + 2;
                        }
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
                        Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) / 2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
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
                                SetColor(DenBackColor, DenTextColor);
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
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(out_den.Substring(SUBJECTWIDTH * counter) + new string(' ', SUBJECTWIDTH - out_den.Substring(SUBJECTWIDTH * counter).Length));
                        }
                        else if (den == SUBJECTWIDTH) Console.Write(out_den);
                        else Console.Write(new string(' ', (SUBJECTWIDTH - den) / 2) + out_den + new string(' ', SUBJECTWIDTH - den - ((SUBJECTWIDTH - den) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
                    }
                }
            }
            Console.WriteLine();
            count++;
        }
    }

    /// <summary>
    /// method to set color of a console
    /// </summary>
    /// <param name="back"> background color </param>
    /// <param name="front"> foreground color </param>
    public static void SetColor(ConsoleColor back, ConsoleColor front)
    {
        Console.BackgroundColor = back;
        Console.ForegroundColor = front;
    }

    /// <summary>
    /// method to print Khai Schedule title
    /// </summary>
    public static void PrintKhai()
    {
        Console.Write("\n\n" + new string(' ', (Console.WindowWidth - 18) / 2) + "|");
        Console.Write(" Расписание ХАИ ");
        Console.WriteLine("|");
        Console.WriteLine('\n');
    }

    /// <summary>
    /// method to print menu
    /// </summary>
    /// <param name="menu"> items of menu </param>
    /// <param name="menu_item"> current item </param>
    /// <param name="theme"> theme of console </param>
    public static void PrintMenu(string[] menu, int menu_item, Theme theme)
    {
        ConsoleColor ConsBackColor = theme.Colors[0];
        ConsoleColor ConsTextColor = theme.Colors[5];
        SetColor(ConsBackColor, ConsTextColor);

        if((menu.Length == 2 || menu.Length == 3) && menu[0].IndexOf("    ") != -1)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu_item == i)
                {
                    if (Array.IndexOf(Theme.examples, ConsTextColor) < 7 || Array.IndexOf(Theme.examples, ConsTextColor) == 8)
                    {
                        SetColor(ConsBackColor, ConsTextColor);
                        if (menu.Length == 3) Console.Write(new string(' ', (Console.WindowWidth - "› Сохранить расписание в текстовый файл".Length) / 2));
                        else Console.Write(new string(' ', (Console.WindowWidth - "› Вернуться в главное меню".Length) / 2));
                        if (ConsBackColor != Theme.examples[7]) SetColor(Theme.examples[7], ConsTextColor);
                        else SetColor(Theme.examples[14], ConsTextColor);
                        Console.WriteLine(menu[i].Trim() + ' ');
                        SetColor(ConsBackColor, ConsTextColor);
                    }
                    else
                    {
                        SetColor(ConsBackColor, ConsTextColor);
                        if (menu.Length == 3) Console.Write(new string(' ', (Console.WindowWidth - "› Сохранить расписание в текстовый файл".Length) / 2));
                        else Console.Write(new string(' ', (Console.WindowWidth - "› Вернуться в главное меню".Length) / 2));
                        if (ConsBackColor != Theme.examples[8]) SetColor(Theme.examples[8], ConsTextColor);
                        else SetColor(Theme.examples[0], ConsTextColor);
                        Console.WriteLine(menu[i].Trim() + ' ');
                        SetColor(ConsBackColor, ConsTextColor);
                    }
                }
                else Console.WriteLine(menu[i]);
            }
        }
        else
        {
            for (int i = 0; i < menu.Length; i++)
            {
                if (menu_item == i)
                {
                    if (Array.IndexOf(Theme.examples, ConsTextColor) < 7 || Array.IndexOf(Theme.examples, ConsTextColor) == 8)
                    {
                        if (ConsBackColor != Theme.examples[7]) SetColor(Theme.examples[7], ConsTextColor);
                        else SetColor(Theme.examples[14], ConsTextColor);
                        Console.WriteLine(menu[i]);
                        SetColor(ConsBackColor, ConsTextColor);
                    }
                    else
                    {
                        if (ConsBackColor != Theme.examples[8]) SetColor(Theme.examples[8], ConsTextColor);
                        else SetColor(Theme.examples[0], ConsTextColor);
                        Console.WriteLine(menu[i]);
                        SetColor(ConsBackColor, ConsTextColor);
                    }
                }
                else Console.WriteLine(menu[i]);
            }
        }
    }
}

class Update
{
    public static void CheckUpdate()
    {
        WebClient webClient = new WebClient();

        try
        {
            if (!webClient.DownloadString("https://github.com/Oyne/Khai-schedule/raw/Updater/current_version.txt").Contains("2.2"))
            {
                if (MessageBox.Show("Доступна новая версия! Хотите установить её прямо сейчас?", "Khai-schedule Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) using (var client = new WebClient())
                    {
                        Process.Start("Updater.exe");
                        Environment.Exit(0);
                    }
            }
        }
        catch
        {

        }
    }
}