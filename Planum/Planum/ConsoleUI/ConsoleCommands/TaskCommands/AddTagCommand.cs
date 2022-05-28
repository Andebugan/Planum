using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class AddTagCommand : ICommand
    {
        ITaskManager _taskManager;
        ITagManager _tagManager;
        IUserManager _userManager;

        public AddTagCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
            _tagManager = tagManager;
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

            if (_tagManager.FindTag(tagId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.AddTagToTask(taskId, tagId);
        }

        public string GetDescription()
        {
            return "adds tag to task";
        }

        public string GetName()
        {
            return "add tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
