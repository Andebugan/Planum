using Planum.Models.BuisnessLogic.Managers;

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
            _userManager.CurrentUser = null;
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
