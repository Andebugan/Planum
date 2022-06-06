using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class RemoveTagCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public RemoveTagCommand(ITaskManager taskManager, IUserManager userManager)
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

            Console.Write("Enter tag id: ");
            input = Console.ReadLine();
            int tagId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out tagId))
            {
                Console.WriteLine("Tag id must be signed integer\n");
                return;
            }
            Console.WriteLine();
            _taskManager.RemoveTagFromTask(taskId, tagId);
        }

        public string GetDescription()
        {
            return "removes tag from task";
        }

        public string GetName()
        {
            return "remove tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
