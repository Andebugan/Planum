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

        protected IEnumerable<string> ParseArgs(string input)
        {
            var inputString = input.Trim();
            var inputArgs = inputString.Split("\"");
            bool insideQuotes = inputString.StartsWith('"');
            IEnumerable<string> args = new List<string>();
            foreach (var inputArg in inputArgs)
            {
                var arg = inputArg.Trim();
                if (insideQuotes && arg.Length != 0)
                    args = args.Append(arg);
                else if (arg.Trim().Length > 0)
                    args = args.Concat(arg.Split(' ').Where(x => x.Length > 0));
                insideQuotes = !insideQuotes;
            }
            return args;
        }

        public void RunConsoleMode()
        {
            Logger.Log(message: "Running console mode");
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (!CommandManager.IsExit)
            {
                var args = ParseArgs(GetInput()); 
                List<string> result = CommandManager.TryExecuteCommand(args);
                PrintResult(result);
            }
            Logger.Log(message: "Console mode exit");
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
