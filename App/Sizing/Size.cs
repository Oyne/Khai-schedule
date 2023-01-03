using System.ComponentModel;

namespace Khai;

    public class Size
    {
        public static int[] SetSize(ConsoleColor[] colors, int tableWidth, int timeWidth)
        {
            int[] arr = new int[2];
            arr[0] = Console.WindowWidth;
            arr[1] = Console.WindowHeight;
            ConsoleKeyInfo keyInfo;
            do
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 1;
                Console.Clear();

                Console.Write("\n\n" + new string(' ', (Console.WindowWidth - "  Настройки размера консоли  ".Length) / 2) + "|");
                Console.Write(" Настройки размера консоли ");
                Console.WriteLine("|");
                Console.WriteLine('\n');
                Instuction();

                Console.Write(new string(' ', (Console.WindowWidth - "Текущий размер: {0} x {1}".Length) / 2) + "Текущий размер: {0} x {1}\n\n", Console.WindowWidth, Console.WindowHeight);
                Theme.PrintTable(colors, tableWidth, timeWidth);

                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        Console.WindowWidth = Math.Max(Console.WindowWidth - 1, tableWidth+2);
                        arr[0] = Console.WindowWidth;
                        break;
                    case ConsoleKey.RightArrow:
                        Console.WindowWidth = Math.Min(Console.WindowWidth + 1, 211);
                        arr[0] = Console.WindowWidth;
                        break;
                    case ConsoleKey.UpArrow:
                        Console.WindowHeight = Math.Max(Console.WindowHeight - 1, 25);
                        arr[1] = Console.WindowHeight;
                        break;
                    case ConsoleKey.DownArrow:
                        Console.WindowHeight = Math.Min(Console.WindowHeight + 1, 49);
                        arr[1] = Console.WindowHeight;
                        break;
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            return arr;
        }

        public static void Instuction()
        {
            Console.Write(new string(' ', (Console.WindowWidth - "← →-управление".Length) / 2));
            Console.WriteLine("↑");
            Console.Write(new string(' ', (Console.WindowWidth - "← → - управление".Length) / 2));
            Console.WriteLine("← →  - управление");
            Console.Write(new string(' ', (Console.WindowWidth - "← →-управление".Length) / 2));
            Console.WriteLine("↓\n");
            Console.Write(new string(' ', (Console.WindowWidth - "Enter - выбор".Length) / 2));
            Console.WriteLine("Enter - выбор\n\n");
        }
    }