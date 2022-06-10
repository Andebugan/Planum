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

        public void Execute(string[] args)
        {
            Serilog.Log.Information("login command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter login: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("login can't be empty!\n");
                Console.ForegroundColor= ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter password: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? password = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password != null)
                        password = password.Remove(0, password.Length - 1);
                }
                password += key.KeyChar;
            }
            Console.WriteLine();

            if (string.IsNullOrEmpty(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("password can't be empty!\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            User? user = _userManager.SignIn(login, password);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("login failed, login or password incorrect\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("login successfull\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public string GetDescription()
        {
            return "logs into system";
        }

        public string GetName()
        {
            return "login";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "login")
                return true;
            return false;
        }
    }
}
