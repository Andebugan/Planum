using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteUserCommand : ICommand
    {
        protected IUserManager _userManager;

        public DeleteUserCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }
        public void Execute()
        {
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            int id = 0;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("id must be signed integer number");
                return;
            }
            _userManager.DeleteUser(id);
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
                return true;
            return false;
        }
    }
}
