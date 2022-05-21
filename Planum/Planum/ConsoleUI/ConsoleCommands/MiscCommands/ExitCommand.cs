using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ExitCommand : ICommand
    {
        public void Execute()
        {
            Environment.Exit(0);
        }

        public string GetDescription()
        {
            return "Closes app";
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
