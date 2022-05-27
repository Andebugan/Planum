using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    internal class AddParentCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public AddParentCommand(ITaskManager taskManager, IUserManager userManager)
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

            Console.WriteLine("Enter parent id: ");
            input = Console.ReadLine();
            int parentId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out parentId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(parentId, _userManager.CurrentUser.Id) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            _taskManager.AddParentToTask(taskId, parentId, _userManager.CurrentUser.Id);
        }

        public string GetDescription()
        {
            return "adds parent to task";
        }

        public string GetName()
        {
            return "add parent";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
