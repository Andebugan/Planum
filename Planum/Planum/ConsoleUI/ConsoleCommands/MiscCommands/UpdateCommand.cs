using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UpdateCommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public UpdateCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void UpdateUser()
        {
            Serilog.Log.Information("Update user command was called");
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            int id;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("id must be signed integer\n");
                return;
            }

            User? user = _userManager.FindUser(id);
            if (user == null)
            {
                Console.WriteLine("User with entered id does not exist\n");
                return;
            }

            Console.Write("Enter new login: ");
            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("login can't be null or empty\n");
                return;
            }
            Console.Write("Enter new password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("password can't be null\n");
                return;
            }
            _userManager.UpdateUser(new User(user.Id, login, password));
            Console.WriteLine();
        }

        public void Execute(string[] args)
        {
            return;
        }

        public string GetDescription()
        {
            return "updates specified object|objects";
        }

        public string GetName()
        {
            return "update user|task|tag";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "update")
                return true;
            return false;
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            else
                return false;
        }
    }
}
