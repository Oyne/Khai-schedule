using Khai;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;

Output.PrintKhai();
int width = 130;
int height = 45;
Console.SetWindowSize(width, height);
while (true)
{
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    Console.OutputEncoding = System.Text.Encoding.Unicode;
    Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

    using var client = new KhaiClient();
    string group = "";
    string name = "";
    int choice;

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
                Console.Write("Введите группу: ");
                group = Console.ReadLine();
                Console.Clear();
                Output.PrintKhai();
                Output.Outputing(group, choice);
            }
            break;
        default:
            {
                Console.Write("Введите имя: ");
                name = Console.ReadLine();
                Console.Clear();
                Output.PrintKhai();
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
        var client = new KhaiClient();
        WeekSchedule Schedule;
        if (choice == 1) Schedule = await client.GetGroupWeekSheduleAsync(str);
        else Schedule = await client.GetStudentWeekSheduleAsync(str);
        //var groupSchedule = await client.GetGroupWeekSheduleAsync(str);
        int count = 0;
        int den;
        int num;
        foreach (var day in Schedule)
        {
            Console.WriteLine("\t" + new string('-', 101));
            Console.Write("\t|");
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', (99 - daysOfWeek[count].Length) / 2) + daysOfWeek[count] + new string(' ', 99 - (99 - daysOfWeek[count].Length) / 2 - daysOfWeek[count].Length));
            Console.ResetColor();
            Console.Write("|\n");
            Console.WriteLine("\t" + new string('-', 101));
            for (int j = 0; j < 5; j++)
            {
                if (j >= day.Classes.Count) break;

                if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator != null)
                {
                    Console.Write("\t|");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("     " + timeOfPairs[j] + "     ");
                    Console.ResetColor();
                    Console.Write("|");
                    if (day.Classes[j].Numerator.RoomNumber == null) num = day.Classes[j].Numerator.Name.Length;
                    else num = day.Classes[j].Numerator.RoomNumber.Length + day.Classes[j].Numerator.Name.Length;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    if (num > 75)
                    {
                        Console.Write(day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name.Substring(0, 75 - day.Classes[j].Denominator.RoomNumber.Length));
                        Console.ResetColor();
                        Console.WriteLine("|");
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.Write(day.Classes[j].Denominator.Name.Substring(75 - day.Classes[j].Denominator.RoomNumber.Length) + new string(' ', 72 - day.Classes[j].Denominator.Name.Substring(75 - day.Classes[j].Denominator.RoomNumber.Length).Length) + "   ");
                    }
                    else if (num == 75) Console.Write(day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name);
                    else Console.Write(new string(' ', (75 - num) / 2) + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 75 - num - ((75 - num) / 2)));
                    Console.ResetColor();
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 101));
                }
                else if (day.Classes[j].Numerator == day.Classes[j].Denominator && day.Classes[j].Numerator == null)
                {
                    Console.Write("\t|");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write("     " + timeOfPairs[j] + "     ");
                    Console.ResetColor();
                    Console.Write("|");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                    Console.ResetColor();
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 101));
                }
                else
                {
                    if (day.Classes[j].Numerator == null)
                    {
                        if (day.Classes[j].Denominator.RoomNumber == null) den = day.Classes[j].Denominator.Name.Length;
                        else den = day.Classes[j].Denominator.RoomNumber.Length + day.Classes[j].Denominator.Name.Length;
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.ResetColor();
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        if (den > 75)
                        {
                            Console.WriteLine(day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name.Substring(0, 75 - day.Classes[j].Denominator.RoomNumber.Length));
                            Console.Write("\t|");
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(new string(' ', 23));
                            Console.ResetColor();
                            Console.Write("|");
                            Console.Write(day.Classes[j].Denominator.Name.Substring(75 - day.Classes[j].Denominator.RoomNumber.Length));
                        }
                        else if (den == 75) Console.Write(day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name);
                        else Console.Write(new string(' ', (75 - den) / 2) + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + new string(' ', 75 - den - ((75 - den) / 2)));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
                    }
                    else if (day.Classes[j].Denominator == null)
                    {
                        if (day.Classes[j].Numerator.RoomNumber == null) num = day.Classes[j].Numerator.Name.Length;
                        else num = day.Classes[j].Numerator.RoomNumber.Length + day.Classes[j].Numerator.Name.Length;
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        if (num > 75)
                        {
                            Console.Write(day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name.Substring(0, 74 - day.Classes[j].Numerator.RoomNumber.Length));
                            Console.ResetColor();
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(new string(' ', 23));
                            Console.ResetColor();
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write(day.Classes[j].Numerator.Name.Substring(74 - day.Classes[j].Numerator.RoomNumber.Length) + new string(' ', 72 - day.Classes[j].Numerator.Name.Substring(75 - day.Classes[j].Numerator.RoomNumber.Length).Length) + "   ");
                        }
                        if (num == 75) Console.Write(day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 75 - num));
                        else Console.Write(new string(' ', (75 - num) / 2) + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 75 - num - ((75 - num) / 2)));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.ResetColor();
                        Console.Write("|");
                        Console.WriteLine(new string('-', 75));
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.Write(new string(' ', (75 - 19) / 2) + "*******************" + new string(' ', 75 - 19 - (75 - 19) / 2));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 101));
                    }
                    else
                    {
                        if (day.Classes[j].Numerator.RoomNumber == null) num = day.Classes[j].Numerator.Name.Length;
                        else num = day.Classes[j].Numerator.RoomNumber.Length + day.Classes[j].Numerator.Name.Length;
                        if (day.Classes[j].Denominator.RoomNumber == null) den = day.Classes[j].Denominator.Name.Length;
                        else den = day.Classes[j].Denominator.RoomNumber.Length + day.Classes[j].Denominator.Name.Length;
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        if (num > 75)
                        {
                            Console.Write(day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name.Substring(0, 74 - day.Classes[j].Numerator.RoomNumber.Length));
                            Console.ResetColor();
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(new string(' ', 23));
                            Console.ResetColor();
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write(day.Classes[j].Numerator.Name.Substring(74 - day.Classes[j].Numerator.RoomNumber.Length) + new string(' ', 72 - day.Classes[j].Numerator.Name.Substring(75 - day.Classes[j].Numerator.RoomNumber.Length).Length) + "   ");
                        }
                        else if (num == 75) Console.Write(day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name);
                        else Console.Write(new string(' ', (75 - num) / 2) + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 75 - num - ((75 - num) / 2)));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("     " + timeOfPairs[j] + "     ");
                        Console.ResetColor();
                        Console.Write("|");
                        Console.WriteLine(new string('-', 76));
                        Console.Write("\t|");
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(new string(' ', 23));
                        Console.ResetColor();
                        Console.Write("|");
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        if (den > 75)
                        {
                            Console.Write(day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name.Substring(0, 74 - day.Classes[j].Denominator.RoomNumber.Length));
                            Console.ResetColor();
                            Console.WriteLine("|");
                            Console.Write("\t|");
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(new string(' ', 23));
                            Console.ResetColor();
                            Console.Write("|");
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(day.Classes[j].Denominator.Name.Substring(75 - day.Classes[j].Denominator.RoomNumber.Length) + new string(' ', 72 - day.Classes[j].Denominator.Name.Substring(75 - day.Classes[j].Denominator.RoomNumber.Length).Length) + "   ");
                        }
                        else if (den == 75) Console.Write(day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name);
                        else Console.Write(new string(' ', (75 - den) / 2) + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + new string(' ', 75 - den - ((75 - den) / 2)));
                        Console.ResetColor();
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
        Console.Write("\n\n\t\t\t\t\t\t|");
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(" Расписание ХАИ ");
        Console.ResetColor();
        Console.WriteLine("|");
        Console.WriteLine('\n');
    }
}