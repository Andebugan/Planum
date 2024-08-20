using System.Collections.Generic;
using Planum.Commands;

namespace Planum
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var commands = new List<ICommand>() {
            };

            var commandManager = new CommandManager(commands);
            var consoleManager = new ConsoleManager(commandManager);

            consoleManager.RunCommandMode(args);
        }
    }
}
