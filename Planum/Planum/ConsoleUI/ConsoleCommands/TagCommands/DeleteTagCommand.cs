using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteTagCommand : ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;

        public DeleteTagCommand(ITagManager tagManager, IUserManager userManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Console.Write("Deleted task id: ");
            string? input = Console.ReadLine();
            int id;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer");
                return;
            }
            _tagManager.DeleteTag(id);
            Console.WriteLine();
        }

        public string GetDescription()
        {
            return "deletes tag";
        }

        public string GetName()
        {
            return "delete tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser == null)
                return false;
            return true;
        }
    }
}
