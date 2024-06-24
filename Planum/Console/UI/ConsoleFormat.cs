using System;

namespace Planum.ConsoleUI.UI
{
    public static class ConsoleFormat
    {
        public static ConsoleColor normalColor = ConsoleColor.White;
        public static ConsoleColor warningColor = ConsoleColor.Yellow;
        public static ConsoleColor errorColor = ConsoleColor.Red;
        public static ConsoleColor messageColor = ConsoleColor.Cyan;

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintMessage(string message, string text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }

        public static void PrintMessage(string message, string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }

        public static void PrintMessage(string message, string text, ConsoleColor color, ConsoleColor messageColor)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = messageColor;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintWarning(string warning, bool newLine = true)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (newLine)
                Console.WriteLine(warning);
            else
                Console.Write(warning);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
