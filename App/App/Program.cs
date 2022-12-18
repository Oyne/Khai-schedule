using Khai;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

int width = 130;
int height = 45;
Console.SetWindowSize(width, height);
ConsoleColor DefaultConsBackColor = Console.BackgroundColor;
ConsoleColor DefaultConsTextColor = Console.ForegroundColor;
//ConsoleColor ConsBackColor = DefaultConsBackColor;
//ConsoleColor ConsTextColor = DefaultConsTextColor;
ConsoleColor ConsBackColor = ConsoleColor.White;
ConsoleColor ConsTextColor = ConsoleColor.Black;

Output.PrintKhai();

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
    Console.Write("1. Поиск по группе\n2. Поиск по имени\n3. Выход\n>>> ");
    while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
    {
        Console.Write("Введите 1, 2 или 3: ");
    }
    if (choice == 3) return;

    switch (choice)
    {
        case 1:
            {
                WeekSchedule groupSch = null;
                while (groupSch == null)
                {
                    Console.Write("Введите группу: ");
                    group = Console.ReadLine();
                    if (int.TryParse(group, out ret) && ret == 3) return;
                    try
                    {
                        groupSch = await client.GetGroupWeekSheduleAsync(group);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.Write("Некорректный ввод, введите группу в формате 515, 525v, 116i1, 516st1\n>>> ");
                    }
                }
                //Console.Clear();
                Console.BackgroundColor = ConsBackColor;
                Console.ForegroundColor = ConsTextColor;
                Console.Clear();
                Output.PrintKhai();
                Console.WriteLine($"\t\t\t\t\t\t   Группа: {group}");
                Output.Outputing(group, choice);
            }
            break;
        default:
            {
                WeekSchedule studentSch = null;
                while (studentSch == null)
                {
                    Console.Write("Введите имя: ");
                    name = Console.ReadLine();
                    if (int.TryParse(name, out ret) && ret == 3) return;
                    try
                    {
                        studentSch = await client.GetStudentWeekSheduleAsync(name);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.Write("Некорректный ввод, введите имя в формате bondarenko-a-o, kuzmichov-i-i\n>>> ");
                    }
                }
                Console.BackgroundColor = ConsBackColor;
                Console.ForegroundColor = ConsTextColor;
                Console.Clear();
                Output.PrintKhai();
                Console.WriteLine($"\t\t\t\t\t      Студент: {name}");
                Output.Outputing(name, choice);
            }
            break;
    }

    Thread.Sleep(1000);
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

    // https://education.khai.edu/union/schedule/student/kuzmichov-i-i
    var StudentSchedule = await client.GetStudentWeekSheduleAsync("kuzmichov-i-i");

    // https://education.khai.edu/union/schedule/lecturer/abramov-k-d-504
    var lecturerSchedule = await client.GetLecturerWeekSheduleAsync("abramov-k-d-504");

    // https://education.khai.edu/union/schedule/group/525v
    var GroupSchedule = await client.GetGroupWeekSheduleAsync("525b");

    Debugger.Break();
}
class Output
{
    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };
    public async static void Outputing(string str, int choice)
    {
        ConsoleColor DefaultBackColor = Console.BackgroundColor;
        ConsoleColor DefaultTextColor = Console.ForegroundColor;
        ConsoleColor TableBackColor = ConsoleColor.White;
        ConsoleColor TableTextColor = ConsoleColor.Black;
        ConsoleColor NumBackColor = ConsoleColor.White;
        ConsoleColor NumTextColor = ConsoleColor.Black;
        ConsoleColor DenBackColor = ConsoleColor.DarkBlue;
        ConsoleColor DenTextColor = ConsoleColor.White;
        ConsoleColor TimeBackColor = ConsoleColor.White;
        ConsoleColor TimeTextColor = ConsoleColor.Black;
        ConsoleColor DayOfWeekBackColor = ConsoleColor.White;
        ConsoleColor DayOfWeekTextColor = ConsoleColor.Black;
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

        foreach (var day in Schedule)
        {
            Console.BackgroundColor = TableBackColor;
            Console.ForegroundColor = TableTextColor;
            Console.WriteLine("\t" + new string('-', 101));
            Console.Write("\t|");
            Console.BackgroundColor = DayOfWeekBackColor;
            Console.ForegroundColor = DayOfWeekTextColor;
            Console.Write(new string(' ', (99 - daysOfWeek[count].Length) / 2) + daysOfWeek[count] + new string(' ', 99 - (99 - daysOfWeek[count].Length) / 2 - daysOfWeek[count].Length));
            Console.BackgroundColor = TableBackColor;
            Console.ForegroundColor = TableTextColor;
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
                    Console.BackgroundColor = TimeBackColor;
                    Console.ForegroundColor = TimeTextColor;
                    Console.Write("     " + timeOfPairs[j] + "     ");
                    Console.BackgroundColor = TableBackColor;
                    Console.ForegroundColor = TableTextColor;
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
                    Console.BackgroundColor = NumBackColor;
                    Console.ForegroundColor = NumTextColor;
                    if (num > 75)
                    {
                        Console.Write(output.Substring(0, 75));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.WriteLine("|");
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.BackgroundColor = NumBackColor;
                        Console.ForegroundColor = NumTextColor;
                        Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                    }
                    else if (num == 75) Console.Write(output);
                    else Console.Write(new string(' ', (75 - num) / 2) + output + new string(' ', 75 - num - ((75 - num) / 2)));
                    Console.BackgroundColor = TableBackColor;
                    Console.ForegroundColor = TableTextColor;
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 101));
                }
                else if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator == null)
                {
                    Console.Write("\t|");
                    Console.BackgroundColor = TimeBackColor;
                    Console.ForegroundColor = TimeTextColor;
                    Console.Write("     " + timeOfPairs[j] + "     ");
                    Console.BackgroundColor = TableBackColor;
                    Console.ForegroundColor = TableTextColor;
                    Console.Write("|");
                    //Console.BackgroundColor = BlankBackColor;
                    //Console.ForegroundColor = BlankTextColor;
                    Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                    Console.BackgroundColor = TableBackColor;
                    Console.ForegroundColor = TableTextColor;
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
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        //Console.BackgroundColor = BlankBackColor;
                        //Console.ForegroundColor = BlankTextColor;
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.BackgroundColor = DenBackColor;
                        Console.ForegroundColor = DenTextColor;
                        if (den > 75)
                        {
                            Console.Write(output.Substring(0, 75));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = TimeBackColor;
                            Console.ForegroundColor = TimeTextColor;
                            Console.Write(new string(' ', 23));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.Write("|");
                            Console.BackgroundColor = DenBackColor;
                            Console.ForegroundColor = DenTextColor;
                            Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                        }
                        else if (den == 75) Console.Write(output);
                        else Console.Write(new string(' ', (75 - den) / 2) + output + new string(' ', 75 - den - ((75 - den) / 2)));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
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
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.BackgroundColor = NumBackColor;
                        Console.ForegroundColor = NumTextColor;
                        if (num > 75)
                        {
                            Console.Write(output.Substring(0, 75));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = TimeBackColor;
                            Console.ForegroundColor = TimeTextColor;
                            Console.Write(new string(' ', 23));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.Write("|");
                            Console.BackgroundColor = NumBackColor;
                            Console.ForegroundColor = NumTextColor;
                            Console.Write(output.Substring(75) + new string(' ', 75 - output.Substring(75).Length));
                        }
                        else if (num == 75) Console.Write(output);
                        else Console.Write(new string(' ', (75 - num) / 2) + output + new string(' ', 75 - num - ((75 - num) / 2)));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.WriteLine(new string('-', 75));
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        //Console.BackgroundColor = BlankBackColor;
                        //Console.ForegroundColor = BlankTextColor;
                        Console.BackgroundColor = DenBackColor;
                        Console.ForegroundColor = DenTextColor;
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
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
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.BackgroundColor = NumBackColor;
                        Console.ForegroundColor = NumTextColor;
                        if (num > 75)
                        {
                            Console.Write(out_num.Substring(0, 75));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = TimeBackColor;
                            Console.ForegroundColor = TimeTextColor;
                            Console.Write(new string(' ', 23));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.Write("|");
                            Console.BackgroundColor = NumBackColor;
                            Console.ForegroundColor = NumTextColor;
                            Console.Write(out_num.Substring(75) + new string(' ', 75 - out_num.Substring(75).Length));
                        }
                        else if (num == 75) Console.Write(out_num);
                        else Console.Write(new string(' ', (75 - num) / 2) + out_num + new string(' ', 75 - num - ((75 - num) / 2)));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        Console.BackgroundColor = TimeBackColor;
                        Console.ForegroundColor = TimeTextColor;
                        Console.Write(new string(' ', 23));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|");
                        Console.BackgroundColor = DenBackColor;
                        Console.ForegroundColor = DenTextColor;
                        if (den > 75)
                        {
                            Console.Write(out_den.Substring(0, 75));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = TimeBackColor;
                            Console.ForegroundColor = TimeTextColor;
                            Console.Write(new string(' ', 23));
                            Console.BackgroundColor = TableBackColor;
                            Console.ForegroundColor = TableTextColor;
                            Console.Write("|");
                            Console.BackgroundColor = DenBackColor;
                            Console.ForegroundColor = DenTextColor;
                            Console.Write(out_den.Substring(75) + new string(' ', 75 - out_den.Substring(75).Length));
                        }
                        else if (den == 75) Console.Write(out_den);
                        else Console.Write(new string(' ', (75 - den) / 2) + out_den + new string(' ', 75 - den - ((75 - den) / 2)));
                        Console.BackgroundColor = TableBackColor;
                        Console.ForegroundColor = TableTextColor;
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
                    }
                }
            }
            Console.WriteLine();
            count++;
        }
    }

    public static void PrintKhai()
    {
        Console.Write("\n\n" + new string(' ', 48) +"|");
        Console.Write(" Расписание ХАИ ");
        Console.WriteLine("|");
        Console.WriteLine('\n');
    }
}