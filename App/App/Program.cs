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
using Themes;
using Minimizing;
using Locking;

//Console title
Console.Title = "Khai schedule";

int width = 150; // width of a console
int height = 45; // heught of a console
Console.SetWindowSize(width, height); // set console size

// code to fix size of a concole
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MINIMIZE, Lock.MF_BYCOMMAND);
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_MAXIMIZE, Lock.MF_BYCOMMAND);
Lock.DeleteMenu(Lock.GetSystemMenu(Lock.GetConsoleWindow(), false), Lock.SC_SIZE, Lock.MF_BYCOMMAND);

//Console.WriteLine("Arrow keys to resize, Enter to quit");
//Console.CursorVisible = false;
//Console.ForegroundColor = ConsoleColor.Red;
//ConsoleKeyInfo keyInfo;

//do
//{
//    Console.CursorLeft = 0;
//    Console.CursorTop = 1;
//    Console.Write("({0}x{1}) ", Console.WindowWidth, Console.WindowHeight);

//    keyInfo = Console.ReadKey();

//    switch (keyInfo.Key)
//    {
//        case ConsoleKey.LeftArrow:
//            Console.WindowWidth = Math.Max(Console.WindowWidth - 1, 20);
//            break;
//        case ConsoleKey.RightArrow:
//            Console.WindowWidth = Math.Min(Console.WindowWidth + 1, 100);
//            break;
//        case ConsoleKey.UpArrow:
//            Console.WindowHeight = Math.Max(Console.WindowHeight - 1, 20);
//            break;
//        case ConsoleKey.DownArrow:
//            Console.WindowHeight = Math.Min(Console.WindowHeight + 1, 48);
//            break;
//    }
//} while (keyInfo.Key != ConsoleKey.Enter);

string settingsFilePath = "C://Khai/settings.json";
string sattings;
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

Theme theme = settings.Theme;
// set background and foreground colors of a console
ConsoleColor DefaultConsBackColor = ConsoleColor.Black;
ConsoleColor DefaultConsTextColor = ConsoleColor.Gray;
ConsoleColor ConsBackColor;
ConsoleColor ConsTextColor;

while (true)
{

    // set the correct ukrainian language output
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

    using var client = new KhaiClient();
    string group = "";
    string name = "";
    string exit = "";
    int choice;
    bool boolean;
    WeekSchedule Schedule = null;

MenuCommand:

    boolean = true;

    ConsBackColor = theme.Colors[0];
    ConsTextColor = theme.Colors[5];
    Console.BackgroundColor = ConsBackColor;
    Console.ForegroundColor = ConsTextColor;
    Console.Clear();

    Output.PrintKhai();

    ConsoleKeyInfo keyInfo;

    Console.Write("""
        1. Поиск по группе <1>
        2. Поиск по имени <2>
        3. Считать расписание из файла <3>
        4. Настроить тему <4>
        5. Выход <Esc>
        >>> 
        """);

    while (boolean)
    {
        keyInfo = Console.ReadKey(true);
        Schedule = null;

        switch (keyInfo.Key)
        {
            case ConsoleKey.D1:
                {
                    while (Schedule == null)
                    {
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
                    Console.BackgroundColor = ConsBackColor;
                    Console.ForegroundColor = ConsTextColor;
                    Console.Clear();
                    Output.PrintKhai();
                    Console.WriteLine(new string(' ', (Console.WindowWidth - 8 - group.Length) / 2) + $"Группа: {group}");
                    await Task.Run(() => Output.Outputing(Schedule, theme.Colors));
                    //Output.Outputing(group, choice);
                    boolean = false;
                }
                break;
            case ConsoleKey.D2:
                {
                    while (Schedule == null)
                    {
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
                    Console.BackgroundColor = ConsBackColor;
                    Console.ForegroundColor = ConsTextColor;
                    Console.Clear();
                    Output.PrintKhai();
                    Console.WriteLine(new string(' ', (Console.WindowWidth - 9 - name.Length) / 2) + $"Студент: {name}");
                    await Task.Run(() => Output.Outputing(Schedule, theme.Colors));
                    //Output.Outputing(name, choice);
                    boolean = false;
                }
                break;
            case ConsoleKey.D3:
                {
                    try
                    {
                        Schedule = FileWork.ReadScheduleFromFile();

                        Console.BackgroundColor = ConsBackColor;
                        Console.ForegroundColor = ConsTextColor;
                        Console.Clear();
                        Output.PrintKhai();
                        await Task.Run(() => Output.Outputing(Schedule, theme.Colors));
                        boolean = false;
                    }
                    catch (FileWasNotFoundException e)
                    {
                        Console.WriteLine("Не удалось прочитать файл.\n");
                    }
                    catch (ScheduleWasNotFoundException e)
                    {
                        Console.WriteLine("Нет сохранённого расписания.\n");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Произошло что-то очень плохое\n");
                    }
                    Console.Write("1. Вернуться в главное меню <1>\n2. Выход <Esc>\n>>> ");
                    boolean = true;
                    while (boolean)
                    {
                        keyInfo = Console.ReadKey(true);
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.D1:
                                {
                                    Console.Clear();
                                    goto MenuCommand;
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
                    }
                }
                break;
            case ConsoleKey.D4:
                {
                    Console.Clear();
                    //ColorMenu.ColorSet(ref theme);
                    int ret = Theme.SetColor(ref theme);
                    if(ret == 1)
                    {
                        FileWork.SaveSettings(settings);
                        Environment.Exit(0);
                    }
                    FileWork.SaveSettings(settings);
                    goto MenuCommand;
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
    }

    //Thread.Sleep(1000);
    Console.Write("1. Сохранить расписание в текстовый файл <1>\n" +
        "2. Вернуться в главное меню <2>\n" +
        "3. Выход <Esc>\n>>> ");
    while (true)
    {
        keyInfo = Console.ReadKey(true);

        switch (keyInfo.Key)
        {
            case ConsoleKey.D1:
                {
                    try
                    {
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (DirectoryDoesNotExistException e)
                    {
                        Console.WriteLine("Путь для сохранения файла не найден");
                        FileWork.CreateAFile("C://Khai/info.json");
                        Console.WriteLine("Путь для сохранения файла создан \"C://Khai/\"");
                        Console.WriteLine("Файл с расписанием создан");
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (FileDoesNotExistException e)
                    {
                        Console.WriteLine("Файл для сохранения не найден");
                        FileWork.CreateAFile("C://Khai/info.json");
                        Console.WriteLine("Файл с расписанием создан по пути \"C://Khai/\"");
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nПроизошло что-то очень плохое");
                        Environment.Exit(0);
                    }
                    Console.WriteLine("Расписание успешно сохранено\n");
                    Console.Write("1. Вернуться в главное меню <1>\n2. Выход <Esc>\n>>> ");
                    boolean  = true;
                    while (boolean)
                    {
                        keyInfo = Console.ReadKey(true);
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.D1:
                                {
                                    Console.Clear();
                                    goto MenuCommand;
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
                    }
                }
                break;
            case ConsoleKey.D2:
                {
                    Console.Clear();
                    goto MenuCommand;
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
    }
   

    Debugger.Break();

}

/// <summary>
/// class for outputing the schedule
/// </summary>
class Output
{
    const int TABLEWIDTH = 110; // set general table size (default: 101)
    const int TIMEWIDTH = 23; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
    const int SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3))


    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };

    /// <summary>
    /// method to print the schedule
    /// </summary>
    /// <param name="Schedule"> parsed variable with schedule data </param>
    /// <returns> print schedule in console </returns>
    public async static Task Outputing(WeekSchedule Schedule, ConsoleColor[] colors)
    {

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
}

//class ColorMenu
//{
//    public static void ColorSet(ref Theme theme)
//    {
//        int ret = 0;
//        bool boolean = true;
//        ConsoleKeyInfo keyInfo;

//    MenuColor:
//        Console.Clear();

//        Console.Write("\n\n" + new string(' ', (Console.WindowWidth - 18) / 2) + "|");
//        Console.Write(" Настройки темы ");
//        Console.WriteLine("|");
//        Console.WriteLine('\n');

//        Console.Write("""
//                1. Изменить цвет фона <1>
//                2. Изменить цвет текста <2>
//                3. Изменить цвет фона знаменателя <3>
//                4. Изменить цвет текста знаменателя <4>
//                5. Установить тему по умолчанию <5>
//                6. Сбросить все цвета <6>
//                7. Вернуться в главное меню <7>
//                8. Выход <Esc>
//                >>> 
//                """
//        );

//        while (boolean == true)
//        {
//            keyInfo = Console.ReadKey(true);
//            switch (keyInfo.Key)
//            {
//                case ConsoleKey.D1:
//                    {
//                        theme.SetBackColor();
//                        goto MenuColor;
//                    }
//                    break;
//                case ConsoleKey.D2:
//                    {
//                        theme.SetFrontColor();
//                        goto MenuColor;
//                    }
//                    break;
//                case ConsoleKey.D3:
//                    {
//                        theme.SetDenBackColor();
//                        goto MenuColor;
//                    }
//                    break;
//                case ConsoleKey.D6:
//                    {
//                        theme.SetDefaultColors();
//                        goto MenuColor;
//                    }
//                    break;
//                case ConsoleKey.D7:
//                    {
//                        boolean = false;
//                    }
//                    break;
//                case ConsoleKey.Tab:
//                    {
//                        MinimizeConsoleWindow();
//                    }
//                    break;
//                case ConsoleKey.Escape:
//                    {
//                        FileWork.SaveSettings(settings);
//                        Environment.Exit(0);
//                    }
//            }
//        }
//    }
//}