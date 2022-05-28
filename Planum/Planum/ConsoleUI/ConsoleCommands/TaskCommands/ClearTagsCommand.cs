using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ClearTagsCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ClearTagsCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Console.Write("Enter task id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Task id must be signed integer");
                return;
            }

            if (_taskManager.FindTask(id) == null)
            {
                Console.WriteLine("Task with specified id does not exist");
                return;
            }

            _taskManager.ClearTags(id);
        }

        public string GetDescription()
        {
            return "clears children of a task";
        }

        public string GetName()
        {
            return "clear children";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
