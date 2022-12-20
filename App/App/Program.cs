using Khai;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

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

// set background and foreground colors of a console
ConsoleColor DefaultConsBackColor = Console.BackgroundColor;
ConsoleColor DefaultConsTextColor = Console.ForegroundColor;
ConsoleColor ConsBackColor = ConsoleColor.Black;
ConsoleColor ConsTextColor = ConsoleColor.Gray;

while (true)
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

    using var client = new KhaiClient();
    string group = "";
    string name = "";
    string exit = "";
    int choice;
    bool boolean;
    WeekSchedule Schedule;

MenuCommand:

    boolean = true;

    Console.BackgroundColor = ConsBackColor;
    Console.ForegroundColor = ConsTextColor;
    Console.Clear();

    Output.PrintKhai();

    ConsoleKeyInfo keyInfo;

    Console.Write("1. Поиск по группе <1>\n" +
        "2. Поиск по имени <2>\n" +
        "3. Считать расписание из файла <3>\n" +
        "4. Выход <Esc>\n>>> "
        );

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
                        Console.Write("Введите группу в формате 325, 525v (525в), 116i1, 432st (432ст), 555vm-2 (555вм/2)\n>>> ");
                        group = Console.ReadLine();
                        if (group == "exit") Environment.Exit(0);
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
                    await Task.Run(() => Output.Outputing(Schedule));
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
                    await Task.Run(() => Output.Outputing(Schedule));
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
                        Console.WriteLine(new string(' ', (Console.WindowWidth - 9 - name.Length) / 2) + $"Студент: {name}");
                        await Task.Run(() => Output.Outputing(Schedule));
                        boolean = false;
                    }
                    catch (FileWasNotFoundException e)
                    {
                        Console.WriteLine("\nНе удалось прочитать файл.");
                    }
                    catch (ScheduleWasNotFoundException e)
                    {
                        Console.WriteLine("\nНет сохранённого расписания.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nПроизошло что-то очень плохое");
                    }
                    Console.Write("1. Вернуться в главное меню <1>\n2. Выход <Esc>\n>>> ");
                    keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.D1:
                            {
                                Console.Clear();
                                goto MenuCommand;
                            }
                            break;
                        case ConsoleKey.Escape:
                            {
                                Environment.Exit(0);
                            }
                            break;
                    }
                }
                break;
            case ConsoleKey.Escape:
                {
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
                        FileWork.CreateAFile();
                        Console.WriteLine("Путь для сохранения файла создан \"C://Khai/\"");
                        Console.WriteLine("Файл с расписанием создан");
                        FileWork.SaveSchduleToFile(Schedule);
                    }
                    catch (FileDoesNotExistException e)
                    {
                        Console.WriteLine("Файл для сохранения не найден");
                        FileWork.CreateAFile();
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
                    keyInfo = Console.ReadKey(true);
                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.D1:
                            {
                                Console.Clear();
                                goto MenuCommand;
                            }
                            break;
                        case ConsoleKey.Escape:
                            {
                                Environment.Exit(0);
                            }
                            break;
                    }
                }
                break;
            case ConsoleKey.D2:
                {
                    Console.Clear();
                    goto MenuCommand;
                }
                break;
            case ConsoleKey.Escape:
                {
                    Environment.Exit(0);
                }
                break;
        }
    }
   

    Debugger.Break();

}
class Output
{
    const int TABLEWIDTH = 110; // set general table size (default: 101)
    const int TIMEWIDTH = 23; // set general time field size (default: 23  (!!!!  NOT LESS THAN 13  !!!!))
    const int SUBJECTWIDTH = TABLEWIDTH - TIMEWIDTH - 3; // set general subject field size (default: 75 (TABLEWIDTH - TIMEWIDTH - 3))


    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };
    public async static Task Outputing(WeekSchedule Schedule)
    {
        ConsoleColor CutrrentBackColor = Console.BackgroundColor;
        ConsoleColor CutrrentTextColor = Console.ForegroundColor;
        ConsoleColor TableBackColor = CutrrentBackColor;           // background color of the table
        ConsoleColor TableTextColor = CutrrentTextColor;           // font color of the table
        ConsoleColor NumBackColor = ConsoleColor.Black;             // background color of the numerator
        ConsoleColor NumTextColor = ConsoleColor.Gray;             // font color of the numerator
        ConsoleColor DenBackColor = ConsoleColor.DarkYellow;          // background color of the denominator
        ConsoleColor DenTextColor = ConsoleColor.Black;             // font color of the denominator
        ConsoleColor TimeBackColor = ConsoleColor.Black;            // background color of the time field
        ConsoleColor TimeTextColor = ConsoleColor.Gray;            // font color of the time field
        ConsoleColor DayOfWeekBackColor = ConsoleColor.Black;       // background color of the day of week field
        ConsoleColor DayOfWeekTextColor = ConsoleColor.Gray;       // font color of the day of week field
        //ConsoleColor BlankBackColor = ConsoleColor.DarkBlue;
        //ConsoleColor BlankTextColor = ConsoleColor.White;

        var client = new KhaiClient();
        //WeekSchedule Schedule;
        //if (choice == 1) Schedule = await client.GetGroupWeekSheduleAsync(str);
        //else Schedule = await client.GetStudentWeekSheduleAsync(str);
        int count = 0;
        int den;
        int num;
        string output;

        foreach (var day in Schedule.AsDays())
        {
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + new string('-', TABLEWIDTH));
            Console.Write(new string(' ', (Console.WindowWidth -TABLEWIDTH)/2) + "|");
            Console.BackgroundColor = DayOfWeekBackColor;
            Console.ForegroundColor = DayOfWeekTextColor;
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
                    Console.Write(new string(' ', (TIMEWIDTH - timeOfPairs[j].Length) /2) + timeOfPairs[j] + new string(' ', TIMEWIDTH - timeOfPairs[j].Length - ((TIMEWIDTH - timeOfPairs[j].Length) / 2)));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|");
                    if (day.Classes[j].Numerator.RoomNumber != null)
                    {
                        output += day.Classes[j].Numerator.RoomNumber + ", ";
                        num += day.Classes[j].Numerator.RoomNumber.Length + 2;
                    }
                    if(day.Classes[j].Numerator.Name != null)
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
                        Console.Write(output.Substring(0, SUBJECTWIDTH));
                        SetColor(TableBackColor, TableTextColor);
                        Console.WriteLine("|");                        
                        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write(new string(' ', TIMEWIDTH));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(NumBackColor, NumTextColor);
                        Console.Write(output.Substring(SUBJECTWIDTH) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH).Length));
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
                        Console.WriteLine(new string('-', SUBJECTWIDTH+1));
                        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', TIMEWIDTH));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(DenBackColor, DenTextColor);
                        if (den > SUBJECTWIDTH)
                        {
                            Console.Write(output.Substring(0, SUBJECTWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', TIMEWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(output.Substring(SUBJECTWIDTH) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH).Length));
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
                            Console.Write(output.Substring(0, SUBJECTWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', TIMEWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(NumBackColor, NumTextColor);
                            Console.Write(output.Substring(SUBJECTWIDTH) + new string(' ', SUBJECTWIDTH - output.Substring(SUBJECTWIDTH).Length));
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
                            Console.Write(out_num.Substring(0, SUBJECTWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', TIMEWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(NumBackColor, NumTextColor);
                            Console.Write(out_num.Substring(SUBJECTWIDTH) + new string(' ', SUBJECTWIDTH - out_num.Substring(SUBJECTWIDTH).Length));
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
                        Console.WriteLine(new string('-', SUBJECTWIDTH+1));
                        Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', TIMEWIDTH));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(DenBackColor, DenTextColor);
                        if (den > SUBJECTWIDTH)
                        {
                            Console.Write(out_den.Substring(0, SUBJECTWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write(new string(' ', (Console.WindowWidth - TABLEWIDTH) / 2) + "|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', TIMEWIDTH));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(out_den.Substring(SUBJECTWIDTH) + new string(' ', SUBJECTWIDTH - out_den.Substring(SUBJECTWIDTH).Length));
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

    public static void SetColor(ConsoleColor back, ConsoleColor front)
    {
        Console.BackgroundColor = back;
        Console.ForegroundColor = front;
    }

    public static void PrintKhai()
    {
        Console.Write("\n\n" + new string(' ', (Console.WindowWidth-18)/2) +"|");
        Console.Write(" Расписание ХАИ ");
        Console.WriteLine("|");
        Console.WriteLine('\n');
    }
}

class Theme
{
    public static void SetColorTheme(ref ConsoleColor[] colors)
    {
        Console.BackgroundColor = colors[0];
        Console.ForegroundColor = colors[1];
    }
}