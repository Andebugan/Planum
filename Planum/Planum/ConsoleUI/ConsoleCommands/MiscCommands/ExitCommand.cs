using Serilog;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Log.Information("Exit command was called");
            Environment.Exit(0);
        }

        public string GetDescription()
        {
            return "Closes application";
        }

        public string GetName()
        {
            return "exit";
        }

        public bool IsAvaliable()
        {
            return true;
        }
    }
}
