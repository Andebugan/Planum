using System.Collections.Generic;
using System.Linq;

namespace Planum.Commands
{
    public class CommandManager
    {
        List<ICommand<ICommandSettings>> Commands { get; set; }

        public CommandManager(IEnumerable<ICommand<ICommandSettings>> commands) => Commands = commands.ToList();

        public List<string> TryExecuteCommand(string[] commandStrings)
        {
            var result = new List<string>();
            var commandStringsList = commandStrings.ToList();
            IEnumerator<string> commandEnumerator = (IEnumerator<string>)(commandStringsList.GetEnumerator());
            commandEnumerator.MoveNext();

            bool match = false;
            foreach (var command in Commands)
            {
                if (command.CheckMatch(ref commandEnumerator))
                {
                    result = command.Execute(ref commandEnumerator);
                    match = true;
                    break;
                }
            }

            if (!match)
                result.Add(ConsoleSpecial.AddStyle("Unable to find matching command", foregroundColor: ConsoleInfoColors.Warning));

            commandEnumerator.Dispose();
            return result;
        }
    }
}
