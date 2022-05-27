using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UpdateUserCommand : ICommand
    {
        protected IUserManager _userManager;

        public UpdateUserCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute()
        {
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            int id;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("id must be signed integer");
                return;
            }

            if (_userManager.FindUser(id) == null)
            {
                Console.WriteLine("User with entered id does not exist");
                return;
            }

            Console.Write("Enter new login: ");
            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("login can't be null or empty");
                return;
            }
            Console.Write("Enter new password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("password can't be null");
                return;
            }
            _userManager.UpdateUser(id, login, password);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "updates user";
        }

        public string GetName()
        {
            return "update user";
        }

        public bool IsAvaliable()
        {
            return true;    
        }
    }
}
