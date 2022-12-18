using Khai;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;


Console.Write("\n\n\t\t\t\t|");
Console.BackgroundColor = ConsoleColor.White;
Console.ForegroundColor = ConsoleColor.Black;
Console.Write(" Расписание ХАИ ");
Console.ResetColor();
Console.WriteLine("|");
Console.WriteLine('\n');
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
                var groupSchedule = await client.GetGroupWeekSheduleAsync(group);
                Output.Outputing(groupSchedule, group);
            }
            break;
        default:
            {
                Console.Write("Введите имя: ");
                name = Console.ReadLine();
                var studentSchedule = await client.GetStudentWeekSheduleAsync(name);

            }
            break;
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
    public async static void Outputing(WeekSchedule weekScheduele, string group)
    {
        var client = new KhaiClient();
        var groupSchedule = await client.GetGroupWeekSheduleAsync(group);
        int count = 0;
        int den;
        int num;
        foreach (var day in groupSchedule)
        {
            Console.WriteLine("\t" + new string('-', 100));
            Console.Write("\t|");
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(new string(' ', 45) + daysOfWeek[count] + new string(' ', 100 - 47 - daysOfWeek[count].Length));
            Console.ResetColor();
            Console.Write("|\n");
            Console.WriteLine("\t" + new string('-', 100));
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
                    Console.Write("        " + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 63 - num) + "   ");
                    Console.ResetColor();
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 100));
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
                    Console.Write(new string(' ', 16) + "******************" + new string(' ', 40));
                    Console.ResetColor();
                    Console.Write("|\n");
                    Console.WriteLine("\t" + new string('-', 100));
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
                        Console.Write(new string(' ', 16) + "******************" + new string(' ', 40));
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
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("        " + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + new string(' ', 63 - den) + "   ");
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 100));
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
                        Console.Write("        " + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 63 - num) + "   ");
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
                        Console.Write(new string(' ', 16) + "******************" + new string(' ', 40));
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 100));
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
                        Console.Write("        " + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + new string(' ', 63 - num) + "   ");
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
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("        " + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + new string(' ', 63 - den) + "   ");
                        Console.ResetColor();
                        Console.Write("|\n");
                        Console.WriteLine("\t" + new string('-', 100));
                    }
                }
            }
            Console.WriteLine();
            count++;
        }
    }
}