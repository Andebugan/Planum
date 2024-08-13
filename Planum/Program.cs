using System;
using Planum.Interface;

namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var test = ConsoleSpecial.AddStyle("test ", TextStyle.Normal, TextForegroundColor.Red, TextBackgroundColor.Default);
            test += ConsoleSpecial.AddStyle("test ", TextStyle.Bold, TextForegroundColor.Blue, TextBackgroundColor.Default);
            test += ConsoleSpecial.AddStyle("test", TextStyle.Dim, TextForegroundColor.Yellow, TextBackgroundColor.Default);
            Console.WriteLine(test);    
        }
    }
}
