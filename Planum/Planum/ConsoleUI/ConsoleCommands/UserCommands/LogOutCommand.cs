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

        public void Execute()
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
            return "log out";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
