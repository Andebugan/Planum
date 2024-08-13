using System.Collections.Generic;

namespace Planum.Commands
{
    public class CommandManager
    {
        public IEnumerable<ICommand> Commands { get; set; } = new List<ICommand>();

        public bool TryExecuteCommand(string[] commandStrings, out List<string> result)
        {
            result = new List<string>();
            IEnumerator<string> commandEnumerator = (IEnumerator<string>)(commandStrings.GetEnumerator());
            if (!commandEnumerator.MoveNext())
            {
                commandEnumerator.Dispose();
                return false;
            }

            foreach (var command in Commands)
            {
                if (command.CheckMatch(ref commandEnumerator))
                {
                    result = command.Execute(ref commandEnumerator);
                    commandEnumerator.Dispose();
                    return true;
                }
            }

            commandEnumerator.Dispose();
            return false;
        }
    }
}
