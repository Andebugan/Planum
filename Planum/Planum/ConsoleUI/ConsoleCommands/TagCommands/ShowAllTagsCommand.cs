using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowAllTagsCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public ShowAllTagsCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Show all tags command was called");
            List<Tag> tags = _tagManager.GetAllTags();
            foreach (Tag tag in tags)
            {
                Console.WriteLine("Tag id: " + tag.Id);
                Console.WriteLine("Tag name: " + tag.Name);
                Console.WriteLine("Tag description: " + tag.Description);
                Console.WriteLine("Tag category: " + tag.Category);
                Console.WriteLine();
            }
        }

        public string GetDescription()
        {
            return "shows all tags";
        }

        public string GetName()
        {
            return "show all tags";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
