using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowTagCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public ShowTagCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            int id = 0;
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer\n");
                return;
            }
            Tag? tag = _tagManager.FindTag(id);

            Console.WriteLine("Tag id: " + tag.Id);
            Console.WriteLine("Tag name: " + tag.Name);
            Console.WriteLine("Tag description: " + tag.Description);
            Console.WriteLine("Tag category: " + tag.Category);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "shows tag";
        }

        public string GetName()
        {
            return "show tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
