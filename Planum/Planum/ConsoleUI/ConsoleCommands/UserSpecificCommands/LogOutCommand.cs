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
            Serilog.Log.Information("Logout command was called");
            _userManager.CurrentUser = null;
            Console.WriteLine();
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
