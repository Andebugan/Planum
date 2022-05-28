using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class RemoveParentCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public RemoveParentCommand(ITaskManager taskManager, IUserManager userManager)
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

            if (_taskManager.FindTask(taskId) == null)
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

            if (_taskManager.FindTask(parentId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.RemoveParentFromTask(taskId, parentId);
        }

        public string GetDescription()
        {
            return "removes parent from task";
        }

        public string GetName()
        {
            return "remove parent";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
