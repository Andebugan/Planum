using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class LogInCommand : ICommand
    {
        protected IUserManager _userManager;

        public LogInCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Login command was called");
            Console.Write("Enter login: ");
            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("login can't be null or empty\n");
                return;
            }
            Console.Write("Enter password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("password can't be null\n");
                return;
            }
            User? user = _userManager.SignIn(login, password);
            if (user == null)
                Console.WriteLine("didn't sign in, login or password incorrect\n");
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "log in if password and login are correct";
        }

        public string GetName()
        {
            return "log in";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return true;
            return false;
        }
    }
}
