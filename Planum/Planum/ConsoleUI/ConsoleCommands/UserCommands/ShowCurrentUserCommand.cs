using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowCurrentUserCommand : ICommand
    {
        protected IUserManager _userManager;

        public ShowCurrentUserCommand(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Show current user command was called");
            if (_userManager.CurrentUser == null)
                return;
            Console.WriteLine("id: " + _userManager.CurrentUser.Id);
            Console.WriteLine("login: " + _userManager.CurrentUser.Login);
            Console.WriteLine("password: " + _userManager.CurrentUser.Password);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "return information about current user";
        }

        public string GetName()
        {
            return "show current user";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
