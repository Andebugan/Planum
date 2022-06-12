using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ArchiveTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ArchiveTaskCommand(ITaskManager taskManager,IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute(string command)
        {
            Serilog.Log.Information("Archive task command was called");
            Console.Write("Enter task id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }
            
            if (_taskManager.FindTask(id) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.ArchiveTask(id);
        }

        public string GetDescription()
        {
            return "archives task\n" +
                "flags:\n" +
                "-all - archive all tasks\n" +
                "-id={value} - specify id of archived task";
        }

        public string GetName()
        {
            return "archive [flags]";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command == "archive")
                return true;
            return false;
        }
    }
}
