using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class CreateTagCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public CreateTagCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            string? input = null;
            Console.Write("Enter tag name: ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Tag name can't be empty");
                return;
            }    
            string name = input;

            Console.Write("Enter tag description: ");
            input = Console.ReadLine();
            string? description = input;

            Console.Write("Enter tag category: ");
            input = Console.ReadLine();
            int category;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out category))
            {
                Console.WriteLine("Tag category must be signed integer");
                return;
            }
            _tagManager.CreateTag(_userManager.CurrentUser.Id, category, name, description);
        }

        public string GetDescription()
        {
            return "creates tag";
        }

        public string GetName()
        {
            return "create tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
