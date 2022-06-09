using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UnarchiveTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public UnarchiveTaskCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute(string[] args)
        {
            Serilog.Log.Information("Unarchive task command was called");
            Console.Write("Enter task id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(id, true) == null)
            {
                Console.WriteLine("Archived task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.UnarchiveTask(id);
        }

        public string GetDescription()
        {
            return "unarchives task\n" +
                "flags:\n" +
                "-all - unarchive all archived tasks\n" +
                "-id=[value] - specify id of unarchived task";
        }

        public string GetName()
        {
            return "unarchive [flags]";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "unarchive")
                return true;
            return false;
        }
    }
}
