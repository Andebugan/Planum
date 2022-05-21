using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class SignUpCommand: ICommand
    {
        protected IUserManager _userManager;

        public SignUpCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute()
        {
            Console.Write("Enter login: ");
            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("login can't be null or empty");
                return;
            }

            if (_userManager.FindUser(login) != null)
            {
                Console.WriteLine("user with this login already exist");
                return;
            }

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("password can't be null");
                return;
            }

            int id = _userManager.CreateUser(login, password);
            _userManager.CurrentUser = _userManager.GetUser(id);
            Console.WriteLine("Created new user with:");
            Console.WriteLine("id: " + id);
            Console.WriteLine("login: " + login);
            Console.WriteLine("password: " + password);
        }

        public string GetDescription()
        {
            return "creates new user";
        }

        public string GetName()
        {
            return "sign up";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return true;
            return false;
        }
    }
}
