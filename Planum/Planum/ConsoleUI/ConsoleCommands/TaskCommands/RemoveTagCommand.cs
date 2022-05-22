using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class RemoveTagCommand : ICommand
    {
        ITaskManager _taskManager;
        ITagManager _tagManager;
        IUserManager _userManager;

        public AddTagCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
            _tagManager = tagManager;
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            throw new System.NotImplementedException();
        }

        public bool IsAvaliable()
        {
            throw new System.NotImplementedException();
        }
    }
}
