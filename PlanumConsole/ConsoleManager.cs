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

            while (!CommandManager.IsExit)
            {
                var inputString = GetInput().Trim();
                var inputArgs = inputString.Split("\"");
                bool insideQuotes = inputString.StartsWith('"');
                IEnumerable<string> args = new List<string>();
                foreach (var inputArg in inputArgs)
                {
                    if (insideQuotes)
                        args = args.Append(inputArg);
                    else if (inputArg.Trim().Length > 0)
                        args = args.Concat(inputArg.Split(' ').Where(x => x.Length > 0));
                    insideQuotes = !insideQuotes;
                }

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
