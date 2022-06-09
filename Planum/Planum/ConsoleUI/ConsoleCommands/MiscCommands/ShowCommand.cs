using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowCommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public ShowCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void ShowCurrentUser()
        {
            Serilog.Log.Information("Show current user command was called");
            if (_userManager.CurrentUser == null)
                return;
            Console.WriteLine("id: " + _userManager.CurrentUser.Id);
            Console.WriteLine("login: " + _userManager.CurrentUser.Login);
            Console.WriteLine("password: " + _userManager.CurrentUser.Password);
            Console.WriteLine();
        }

        public void ShowAllUsers()
        {
            Serilog.Log.Information("Show all users command was called");
            List<User> users = _userManager.GetAllUsers();
            foreach (User user in users)
            {
                Console.WriteLine("login: " + user.Login);
            }
            Console.WriteLine();
        }

        public void Execute(string[] args)
        {
            return;
        }

        public string GetDescription()
        {
            return "displays specified object|objects\n" +
                "flags:\n" +
                "-all - display all objects, used by default\n" +
                "-l [options] - display as a list with list options" +
                "-id=[value] - specify id of displayed object, value of said id must be signed integer";
        }

        public string GetName()
        {
            return "show [options] user|task|tag";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "show")
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
