using System;
using System.Collections.Generic;
using System.Linq;

namespace Planum.Commands
{
    public class ConsoleManager 
    {
        protected CommandManager CommandManager { get; set; }

        public ConsoleManager(CommandManager commandManager)
        {
            CommandManager = commandManager;
        }

        void PrintResult(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                Console.WriteLine(line);
        }

        string GetInput()
        {
            Console.Write("\n> ");
            var line = Console.ReadLine();
            return line is null ? "" : line;
        }

        public void RunConsoleMode()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                var input = GetInput(); // Split first by " then by ` `
            }
        }

        public void RunCommandMode(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<string> result = CommandManager.TryExecuteCommand(args);
            PrintResult(result);
        }
    }
}
