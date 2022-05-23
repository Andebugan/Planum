using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class ClearChildrenCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ClearChildrenCommand(ITaskManager taskManager, IUserManager userManager)
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

            if (_taskManager.FindTask(id, _userManager.CurrentUser.Id) == null)
            {
                Console.WriteLine("Task with specified id does not exist");
                return;
            }
            _taskManager.ClearChildren(id, _userManager.CurrentUser.Id);
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
