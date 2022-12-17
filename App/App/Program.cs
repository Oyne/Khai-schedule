using Khai;
using System;
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);

using var client = new KhaiClient();
string group = "";
string name = "";
int choice;

Console.WriteLine("\n\n\t\tРасписание ХАИ\n");
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

class Output
{
    static string[] timeOfPairs = { "08:00 - 09:35", "09:50 - 11:25", "11:55 - 13:30", "13:45 - 15:20", "15:35 - 17:10" };
    static string[] daysOfWeek = { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця" };
    public async static void Outputing(WeekSchedule weekScheduele, string group)
    {
        var client = new KhaiClient();
        var groupSchedule = await client.GetGroupWeekSheduleAsync(group);
        int count = 0;
        foreach (var day in groupSchedule)
        {
            Console.WriteLine("\t" + new string('-', 100));
            Console.WriteLine($"\t|\t\t\t\t\t\t{daysOfWeek[count]}\t\t\t\t\t   |");
            Console.WriteLine("\t" + new string('-', 100));
            for (int j = 0; j < 5; j++)
            {
                if (j == 4 && day.Classes.Count == 4) break;
                Console.Write("\t|\t" + timeOfPairs[j] + "\t|\t");
                if (day.Classes[j].Numerator == day.Classes[j].Denominator)
                {
                    Console.WriteLine("\t" + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + "\t\t\t\t   |");
                    Console.WriteLine("\t" + new string('-', 100));
                }
                else
                {
                    if (day.Classes[j].Numerator == null)
                    {
                        Console.WriteLine("\t******************\t\t\t\t\t   |");
                        Console.WriteLine("\t|\t\t\t|" + new string('-', 75));
                        Console.Write("\t|\t\t\t|\t\t" + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + "\t\t\t\t   |");
                    }
                    else if (day.Classes[j].Denominator == null)
                    {
                        Console.Write("\t|\t\t\t|\t\t" + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + "\t\t\t\t   |");
                        Console.WriteLine("\t|\t\t\t|" + new string('-', 75));
                        Console.WriteLine("\t******************\t\t\t\t   |");
                    }
                    else
                    {
                        Console.Write("\t|\t\t\t|\t\t" + day.Classes[j].Numerator.RoomNumber + day.Classes[j].Numerator.Name + "\t\t\t\t   |");
                        Console.WriteLine("\t|\t\t\t|" + new string('-', 75));
                        Console.Write("\t|\t\t\t|\t\t" + day.Classes[j].Denominator.RoomNumber + day.Classes[j].Denominator.Name + "\t\t\t\t   |");
                    }
                }
            }
            Console.WriteLine();
            count++;
        }
    }
}