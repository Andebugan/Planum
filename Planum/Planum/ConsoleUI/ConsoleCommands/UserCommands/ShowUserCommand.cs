using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowUserCommand : ICommand
    {
        protected IUserManager _userManager;

        public ShowUserCommand(IUserManager userManager)
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
            User? user = _userManager.FindUser(id);
            if (user == null)
            {
                Console.WriteLine("user with this id does not exist");
                return;
            }
            Console.WriteLine("id: " + user.Id);
            Console.WriteLine("login: " + user.Login);
            Console.WriteLine("password: " + user.Password);
        }

        public string GetDescription()
        {
            return "shows user with specified id";
        }

        public string GetName()
        {
            return "show user";
        }

        public bool IsAvaliable()
        {
            return true;
        }
    }
}
