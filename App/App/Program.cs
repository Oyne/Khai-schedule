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

int width = 120;
int height = 45;
Console.SetWindowSize(width, height);

DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MINIMIZE, MF_BYCOMMAND);
DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);


ConsoleColor DefaultConsBackColor = Console.BackgroundColor;
ConsoleColor DefaultConsTextColor = Console.ForegroundColor;
ConsoleColor ConsBackColor = ConsoleColor.White;
ConsoleColor ConsTextColor = ConsoleColor.Black;

while (true)
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

    using var client = new KhaiClient();
    string group = "";
    string name = "";
    int choice;
    int ret = 0;

MenuCommand:

    Console.BackgroundColor = ConsBackColor;
    Console.ForegroundColor = ConsTextColor;
    Console.Clear();

    Output.PrintKhai();

    Console.Write("1. Поиск по группе\n2. Поиск по имени\n3. Выход\n>>> ");
    while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
    {
        Console.Write("Введите 1, 2 или 3: ");
    }
    if (choice == 3) Environment.Exit(0);

    switch (choice)
    {
        case 1:
            {
                WeekSchedule groupSch = null;
                while (groupSch == null)
                {
                    Console.Write("Введите группу в формате 325, 525v (525в), 116i1, 432st (432ст), 555vm-2 (555вм/2)\n>>> ");
                    group = Console.ReadLine();
                    if (group == "exit") Environment.Exit(0);
                    try
                    {
                        groupSch = await client.GetGroupWeekSheduleAsync(group);
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
                Console.WriteLine($"\t\t\t\t\t\t   Группа: {group}");
                await Task.Run(() => Output.Outputing(group, choice));
                //Output.Outputing(group, choice);
            }
            break;
        default:
            {
                WeekSchedule studentSch = null;
                while (studentSch == null)
                {
                    Console.Write("Введите имя в формате bondarenko-a-o, kuzmichov-i-i\n>>> ");
                    name = Console.ReadLine();
                    if (name == "exit") Environment.Exit(0);
                    try
                    {
                        studentSch = await client.GetStudentWeekSheduleAsync(name);
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
                Console.WriteLine($"\t\t\t\t\t      Студент: {name}");
                await Task.Run(() => Output.Outputing(name, choice));
                //Output.Outputing(name, choice);
            }
            break;
    }

    //Thread.Sleep(1000);
    Console.Write("1. Сохранить расписание в текстовый файл\n2. Вернуться в главное меню\n3. Выход\n>>> ");
    while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
    {
        Console.Write("Введите 1, 2 или 3: ");
    }
    if (choice == 3) return;
    else if (choice == 2)
    {
        Console.Clear();
        goto MenuCommand;
    }
    else
    {
        Console.WriteLine("Данная функция ещё не доступна");
        Console.Write("1. Вернуться в главное меню\n2. Выход\n>>> ");
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 2)
        {
            Console.Write("Введите 1 или 2: ");
        }
        if (choice == 2) return;
        else if (choice == 1)
        {
            Console.Clear();
            goto MenuCommand;
        }
    }
    Debugger.Break();
}
class Output
{
    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };
    public async static Task Outputing(string str, int choice)
    {
        ConsoleColor DefaultBackColor = Console.BackgroundColor;
        ConsoleColor DefaultTextColor = Console.ForegroundColor;
        ConsoleColor TableBackColor = ConsoleColor.White;           // background color of the table
        ConsoleColor TableTextColor = ConsoleColor.Black;           // font color of the table
        ConsoleColor NumBackColor = ConsoleColor.White;             // background color of the numerator
        ConsoleColor NumTextColor = ConsoleColor.Black;             // font color of the numerator
        ConsoleColor DenBackColor = ConsoleColor.DarkBlue;          // background color of the denominator
        ConsoleColor DenTextColor = ConsoleColor.White;             // font color of the denominator
        ConsoleColor TimeBackColor = ConsoleColor.White;            // background color of the time field
        ConsoleColor TimeTextColor = ConsoleColor.Black;            // font color of the time field
        ConsoleColor DayOfWeekBackColor = ConsoleColor.White;       // background color of the day of week field
        ConsoleColor DayOfWeekTextColor = ConsoleColor.Black;       // font color of the day of week field
        //ConsoleColor BlankBackColor = ConsoleColor.DarkBlue;
        //ConsoleColor BlankTextColor = ConsoleColor.White;

        var client = new KhaiClient();
        WeekSchedule Schedule;
        if (choice == 1) Schedule = await client.GetGroupWeekSheduleAsync(str);
        else Schedule = await client.GetStudentWeekSheduleAsync(str);
        int count = 0;
        int den;
        int num;
        string output;

        foreach (var day in Schedule.AsDays())
        {
            SetColor(TableBackColor, TableTextColor);
            Console.WriteLine("\t" + new string('-', 101));
            Console.Write("\t|");
            Console.BackgroundColor = DayOfWeekBackColor;
            Console.ForegroundColor = DayOfWeekTextColor;
            Console.Write(new string(' ', (99 - daysOfWeek[count].Length) / 2) + daysOfWeek[count] + new string(' ', 99 - (99 - daysOfWeek[count].Length) / 2 - daysOfWeek[count].Length));
            SetColor(TableBackColor, TableTextColor);
            Console.Write("|\n");
            Console.WriteLine("\t" + new string('-', 101));
            for (int j = 0; j < 5; j++)
            {
                if (j >= day.Classes.Count) break;
                output = "";
                num = 0;
                den = 0;
                if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator != null)
                {
                    Console.Write("\t|");
                    SetColor(TimeBackColor, TimeTextColor);
                    Console.Write("     " + timeOfPairs[j] + "     ");
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
                    if (num > 75)
                    {
                        Console.Write(output.Substring(0, 75));
                        SetColor(TableBackColor, TableTextColor);
                        Console.WriteLine("|");
                        Console.Write("\t|");
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(NumBackColor, NumTextColor);
                        Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                    }
                    else if (num == 75) Console.Write(output);
                    else Console.Write(new string(' ', (75 - num) / 2) + output + new string(' ', 75 - num - ((75 - num) / 2)));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 101));
                }
                else if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator == null)
                {
                    Console.Write("\t|");
                    SetColor(TimeBackColor, TimeTextColor);
                    Console.Write("     " + timeOfPairs[j] + "     ");
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|");
                    //Console.BackgroundColor = BlankBackColor;
                    //Console.ForegroundColor = BlankTextColor;
                    Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                    SetColor(TableBackColor, TableTextColor);
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 101));
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
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        //Console.BackgroundColor = BlankBackColor;
                        //Console.ForegroundColor = BlankTextColor;
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(DenBackColor, DenTextColor);
                        if (den > 75)
                        {
                            Console.Write(output.Substring(0, 75));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', 23));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                        }
                        else if (den == 75) Console.Write(output);
                        else Console.Write(new string(' ', (75 - den) / 2) + output + new string(' ', 75 - den - ((75 - den) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
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
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(NumBackColor, NumTextColor);
                        if (num > 75)
                        {
                            Console.Write(output.Substring(0, 75));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', 23));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(NumBackColor, NumTextColor);
                            Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                        }
                        else if (num == 75) Console.Write(output);
                        else Console.Write(new string(' ', (75 - num) / 2) + output + new string(' ', 75 - num - ((75 - num) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        Console.WriteLine(new string('-', 75));
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        //Console.BackgroundColor = BlankBackColor;
                        //Console.ForegroundColor = BlankTextColor;
                        SetColor(DenBackColor, DenTextColor);
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
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
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(NumBackColor, NumTextColor);
                        if (num > 75)
                        {
                            Console.Write(out_num.Substring(0, 75));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', 23));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(NumBackColor, NumTextColor);
                            Console.Write(out_num.Substring(75) + new string(' ', 75 - out_num.Substring(75).Length));
                        }
                        else if (num == 75) Console.Write(out_num);
                        else Console.Write(new string(' ', (75 - num) / 2) + out_num + new string(' ', 75 - num - ((75 - num) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        SetColor(TimeBackColor, TimeTextColor);
                        Console.Write(new string(' ', 23));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|");
                        SetColor(DenBackColor, DenTextColor);
                        if (den > 75)
                        {
                            Console.Write(out_den.Substring(0, 75));
                            SetColor(TableBackColor, TableTextColor);
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            SetColor(TimeBackColor, TimeTextColor);
                            Console.Write(new string(' ', 23));
                            SetColor(TableBackColor, TableTextColor);
                            Console.Write("|");
                            SetColor(DenBackColor, DenTextColor);
                            Console.Write(out_den.Substring(75) + new string(' ', 75 - out_den.Substring(75).Length));
                        }
                        else if (den == 75) Console.Write(out_den);
                        else Console.Write(new string(' ', (75 - den) / 2) + out_den + new string(' ', 75 - den - ((75 - den) / 2)));
                        SetColor(TableBackColor, TableTextColor);
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
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
        Console.Write("\n\n" + new string(' ', 48) +"|");
        Console.Write(" Расписание ХАИ ");
        Console.WriteLine("|");
        Console.WriteLine('\n');
    }
}