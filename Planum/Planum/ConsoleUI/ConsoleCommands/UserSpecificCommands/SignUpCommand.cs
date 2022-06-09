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

        public void Execute(string[] args)
        {
            Serilog.Log.Information("sign up command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter login: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nlogin can't be empty\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (_userManager.FindUser(login) != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nuser with this login already exist\n");
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
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password != null)
                        password = password.Remove(0, password.Length - 1);
                }
                password += key.KeyChar;
            }

            if (string.IsNullOrEmpty(password))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\npassword can't be null\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("repeat password: ");
            Console.ForegroundColor = ConsoleColor.White;

            string? checkPassword = null;
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
                checkPassword += key.KeyChar;
            }
            Console.WriteLine();

            if (password != checkPassword)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\npasswords do not match\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            int id = _userManager.CreateUser(login, password);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nsuccessfully created new user profile\n");
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
