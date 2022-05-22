using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

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
            List<Tag> tags = _tagManager.GetAllTags(_userManager.CurrentUser.Id);
            foreach (Tag tag in tags)
                _tagManager.DeleteTag(tag.Id);
            Console.WriteLine();
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
