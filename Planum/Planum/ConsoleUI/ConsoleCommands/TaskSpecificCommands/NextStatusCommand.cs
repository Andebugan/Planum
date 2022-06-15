using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Planum.ConsoleUI.ConsoleCommands
{
    internal class NextStatusCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;
        ITagManager _tagManager;

        public NextStatusCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
            _tagManager = tagManager;
        }

        public void Execute(string command)
        {
            string[] args = command.Split(' ');

            if (args[args.Length - 1] == "task" && args.Length == 3)
            {
                if (args[1].Substring(0, 4) == "-id=")
                {
                    int id = -1;
                    if (int.TryParse(args[1].Substring(4), out id) && id >= 0)
                    {
                        _taskManager.NextStatus(id);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("moved status successfully\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "if task has status queue, moves current status index one step forwards (right)";
        }

        public string GetName()
        {
            return "next -id={value} task";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "next")
                return true;
            return false;
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            else
                return false;
        }
    }
}
