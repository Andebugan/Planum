using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public DeleteTaskCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Delete task command was called");
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
            _taskManager.DeleteTask(id);
        }

        public string GetDescription()
        {
            return "deletes task";
        }

        public string GetName()
        {
            return "delete task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
