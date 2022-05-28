using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteUserCommand : ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public DeleteUserCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }
        public void Execute()
        {
            _userManager.DeleteUser(_taskManager, _tagManager);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "Deletes user with specified id";
        }

        public string GetName()
        {
            return "delete user";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
