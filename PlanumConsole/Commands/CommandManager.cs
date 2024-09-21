using Planum.Logger;

namespace Planum.Console.Commands
{
    public class CommandManager
    {
        List<ICommand> Commands { get; set; }
        ILoggerWrapper Logger { get; set; }
        ExitCommand ExitCommand { get; set; }

        public bool IsExit 
        {
            get => ExitCommand.Triggered;
        }

        public CommandManager(IEnumerable<ICommand> commands, ExitCommand exitCommand, ILoggerWrapper logger)
        {
            Commands = commands.ToList();
            Logger = logger;
            ExitCommand = exitCommand;
        }

        public List<string> TryExecuteCommand(IEnumerable<string> commandStrings)
        {
            Logger.Log(message: "Trying to execute command");
            var result = new List<string>();

            if (commandStrings.Count() == 0)
            {
                Logger.Log(message: "Empty input");
                return result;
            }

            var commandStringsList = commandStrings.ToList();
            IEnumerator<string> commandEnumerator = (IEnumerator<string>)(commandStringsList.GetEnumerator());
            var moveNext = commandEnumerator.MoveNext();

            bool match = false;
            foreach (var command in Commands)
            {
                if (command.CheckMatch(commandEnumerator.Current))
                {
                    result = command.Execute(ref commandEnumerator);
                    match = true;
                    break;
                }
            }

            if (!match)
            {
                Logger.Log(message: "Matching command not found");
                result.Add(ConsoleSpecial.AddStyle("Unable to find matching command", foregroundColor: ConsoleInfoColors.Warning));
            }

            commandEnumerator.Dispose();
            Logger.Log(message: "Command execution complete");
            return result;
        }
    }
}
