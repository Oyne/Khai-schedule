namespace Khai;

public class TableSize
{
    public static int[] SetTableSize(ConsoleColor[] colors, int tableWidth, int timeWidth)
    {
        int[] arr = new int[2];
        arr[0] = tableWidth;
        arr[1] = timeWidth;
        ConsoleKeyInfo keyInfo;
        //keyInfo = Console.ReadKey();

        do
        {
                Console.Clear();

                Console.Write("\n\n" + new string(' ', (Console.WindowWidth - "  Настройки размера таблицы  ".Length) / 2) + "|");
                Console.Write(" Настройки размера таблицы ");
                Console.WriteLine("|");
                Console.WriteLine('\n');
                Instuction();

                Console.Write(new string(' ', (Console.WindowWidth - "Текущий размер: {0} x {1}".Length) / 2) + "Текущий размер: {0} x {1}\n\n", tableWidth, timeWidth);
                Theme.PrintTable(colors, tableWidth, timeWidth);

                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        tableWidth = Math.Max(tableWidth - 1, 72);
                        arr[0] = tableWidth;
                        break;
                    case ConsoleKey.RightArrow:
                        tableWidth = Math.Min(tableWidth + 1, 211);
                        arr[0] = tableWidth;
                        break;
                    case ConsoleKey.UpArrow:
                        timeWidth = Math.Min(timeWidth + 1, 50);
                        arr[1] = timeWidth;
                        break;
                    case ConsoleKey.DownArrow:
                        timeWidth = Math.Max(timeWidth - 1, 13);
                        arr[1] = timeWidth;
                        break;
                }
        } while (keyInfo.Key != ConsoleKey.Enter);

            return arr;
    }

    public static void Instuction()
    {
        Console.Write(new string(' ', (Console.WindowWidth - "↓-изменение длины области времени".Length) / 2));
        Console.WriteLine("↑ - изменение длины области времени");
        Console.Write(new string(' ', (Console.WindowWidth - "↓-изменение длины области времени".Length) / 2));
        Console.WriteLine("↓");
        Console.Write(new string(' ', (Console.WindowWidth - "← → - изменение общей длины таблицы".Length) / 2));
        Console.WriteLine("← → - изменение общей длины таблицы\n");
        Console.Write(new string(' ', (Console.WindowWidth - "Enter - выбор".Length) / 2));
        Console.WriteLine("Enter - выбор\n\n");
    }
}