using Serilog;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ExitCommand : ICommand
    {
        public void Execute(string command)
        {
            string[] args = command.Split(' ');
            Log.Information("Exit command was called");
            Environment.Exit(0);
        }

        public string GetDescription()
        {
            return "closes application";
        }

        public string GetName()
        {
            return "exit";
        }

        public bool IsAvaliable()
        {
            return true;
        }

        public bool IsCommand(string command)
        {
            if (command == "exit")
                return true;
            return false;
        }
    }
}
