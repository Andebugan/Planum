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

        public void Execute(string command)
        {
            Serilog.Log.Information("sign up command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter login: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("login can't be empty\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (_userManager.FindUser(login) != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("user with this login already exist\n");
                Console.ForegroundColor = ConsoleColor.White;
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
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password != null && password.Length > 0)
                        password = password.Substring(0, password.Length - 1);
                }
                else
                    password += key.KeyChar;
            }
            Console.WriteLine();

            if (string.IsNullOrEmpty(password))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("password can't be null\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("repeat password: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? checkPassword = null;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password != null && password.Length > 0)
                        password = password.Substring(0, password.Length - 1);
                }
                else
                    password += key.KeyChar;
            }
            Console.WriteLine();

            if (password != checkPassword)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("passwords do not match\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            int id = _userManager.CreateUser(login, password);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("successfully created new user profile\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "creates new user";
        }

        public string GetName()
        {
            return "signup";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "signup")
                return true;
            return false;
        }
    }
}
