using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteAllUsersCommand : ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public DeleteAllUsersCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }
        public void Execute()
        {
            List<User> users = _userManager.GetAllUsers();
            foreach (User user in users)
            {
                _userManager.DeleteUser(user.Id, _taskManager, _tagManager);
            }
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "Deletes all users";
        }

        public string GetName()
        {
            return "delete all users";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
