using System.Collections.Generic;
using System.Linq;
using Planum.Console.Commands;
using Planum.Logger;

namespace Planum.Console
{
    public class ConsoleManager 
    {
        protected CommandManager CommandManager { get; set; }
        protected ILoggerWrapper Logger { get; set; }

        public ConsoleManager(CommandManager commandManager, ILoggerWrapper logger)
        {
            CommandManager = commandManager;
            Logger = logger;
        }

        void PrintResult(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                System.Console.WriteLine(line);
        }

        string GetInput()
        {
            System.Console.Write("> ");
            var line = System.Console.ReadLine();
            return line is null ? "" : line;
        }

        public void RunConsoleMode()
        {
            Logger.Log(message: "Running console mode");
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                var input = GetInput().Trim();
                if (input == string.Empty)
                    continue;
                var quoteSplit = input.Split("\"");
                bool quotes = false;
                if (quoteSplit.Length == 0)
                    continue;
                if (quoteSplit.First() == string.Empty)
                    quotes = true;
                IEnumerable<string> args = new List<string>();
                foreach (var split in quoteSplit)
                {
                    if (quotes)
                        args = args.Append(split);
                    else
                        args = args.Concat(split.Split(' '));
                    quotes = !quotes;
                }
                List<string> result = CommandManager.TryExecuteCommand(args);
                PrintResult(result);
            }
        }

        public void RunCommandMode(string[] args)
        {
            Logger.Log(message: "Running command mode");
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            List<string> result = CommandManager.TryExecuteCommand(args);
            PrintResult(result);
            Logger.Log(message: "Printing results");
        }
    }
}
