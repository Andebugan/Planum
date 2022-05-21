using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteAllUsersCommand : ICommand
    {
        protected IUserManager _userManager;

        public DeleteAllUsersCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }
        public void Execute()
        {
            List<User> users = _userManager.GetAllUsers();
            foreach (User user in users)
            {
                _userManager.DeleteUser(user.Id);
            }
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
