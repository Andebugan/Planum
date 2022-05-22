using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ArchiveTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ArchiveTaskCommand(ITaskManager taskManager,IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public string GetDescription()
        {
            return "archives task";
        }

        public string GetName()
        {
            return "archive task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
