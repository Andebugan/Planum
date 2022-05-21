using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowAllUsersCommand : ICommand
    {
        protected IUserManager _userManager;

        public ShowAllUsersCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute()
        {
            List<User> users = _userManager.GetAllUsers();
            foreach (User user in users)
            {
                Console.WriteLine("id: " + user.Id);
                Console.WriteLine("login: " + user.Login);
                Console.WriteLine("password: " + user.Password);
                Console.WriteLine();
            }
        }

        public string GetDescription()
        {
            return "shows all users";
        }

        public string GetName()
        {
            return "show all users";
        }

        public bool IsAvaliable()
        {
            return true;
        }
    }
}
