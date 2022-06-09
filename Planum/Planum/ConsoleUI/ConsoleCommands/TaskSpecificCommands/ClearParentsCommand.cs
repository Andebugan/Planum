using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ClearParentsCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ClearParentsCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Clear parents command was called");
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
            _taskManager.ClearParents(id);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "clears parents of a task";
        }

        public string GetName()
        {
            return "clear parents";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
