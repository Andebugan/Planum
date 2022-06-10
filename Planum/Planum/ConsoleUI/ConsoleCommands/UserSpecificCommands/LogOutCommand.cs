using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class LogOutCommand : ICommand
    {
        protected IUserManager _userManager;

        public LogOutCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute(string[] args)
        {
            Serilog.Log.Information("logout command was called");
            _userManager.CurrentUser = null;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("logged out of profile\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "log out from user";
        }

        public string GetName()
        {
            return "logout";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "logout")
                return true;
            return false;
        }
    }
}
