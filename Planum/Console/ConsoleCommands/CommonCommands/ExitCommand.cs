using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.CommonCommands
{
    public class ExitCommand : BaseCommand
    {
        public bool exit = false;

        public ExitCommand(): base("exit", "exits the application", "") { }

        public override void Execute(List<string> args)
        {
            exit = true;
        }
    }
}
