using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class RemoveChildCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public RemoveChildCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            string? input;
            Console.Write("Enter task id: ");
            input = Console.ReadLine();
            int taskId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(taskId, _userManager.CurrentUser.Id) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.WriteLine("Enter child id: ");
            input = Console.ReadLine();
            int childId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out childId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(childId, _userManager.CurrentUser.Id) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            _taskManager.RemoveChildFromTask(taskId, childId, _userManager.CurrentUser.Id);
        }

        public string GetDescription()
        {
            return "removes child from a task";
        }

        public string GetName()
        {
            return "remove child";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
