using System.Collections.Generic;
using System.Linq;
using Planum.Logger;

namespace Planum.Commands
{
    public class CommandManager
    {
        List<ICommand> Commands { get; set; }
        ILoggerWrapper Logger { get; set; }

        public CommandManager(IEnumerable<ICommand> commands, ILoggerWrapper logger)
        {
            Commands = commands.ToList();
            Logger = logger;
        }

        public List<string> TryExecuteCommand(IEnumerable<string> commandStrings)
        {
            Logger.Log(message: "Searching for matching command"); 
            var result = new List<string>();
            var commandStringsList = commandStrings.ToList();
            IEnumerator<string> commandEnumerator = (IEnumerator<string>)(commandStringsList.GetEnumerator());
            commandEnumerator.MoveNext();

            bool match = false;
            foreach (var command in Commands)
            {
                if (command.CheckMatch(commandEnumerator.Current))
                {
                    commandEnumerator.MoveNext();
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
            return result;
        }
    }
}
