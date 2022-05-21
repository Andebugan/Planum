using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteAllTagsCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public DeleteAllTagsCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            
        }

        public string GetDescription()
        {
            return "deletes all tags";
        }

        public string GetName()
        {
            return "delete all tags";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
