using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UpdateTagCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public UpdateTagCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Update tag command was called");
            int id = 0;
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer\n");
                return;
            }

            Tag? tag = _tagManager.FindTag(id);
            if (tag == null)
            {
                Console.WriteLine("User with specified id does not exist\n");
                return;
            }

            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name can't be null or zero\n");
                return;
            }

            Console.Write("Enter descriptions: ");
            string? description = Console.ReadLine();
            if (description == null)
                description = "";

            Console.Write("Enter category: ");
            input = Console.ReadLine();
            int category;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out category))
            {
                Console.WriteLine("Category must be signed integer\n");
                return;
            }
            Console.WriteLine();

            _tagManager.UpdateTag(new Tag(tag.Id, tag.UserId, category, name, description));
        }

        public string GetDescription()
        {
            return "updates tag parametes";
        }

        public string GetName()
        {
            return "update tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
