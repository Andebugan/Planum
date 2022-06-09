using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteCommand: ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public DeleteCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void DeleteUser()
        {
            Serilog.Log.Information("Delete user command was called");
            _userManager.DeleteUser(_taskManager, _tagManager);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("User was successfully deleted\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Execute(string[] args)
        {
            return;
        }

        public string GetDescription()
        {
            return "deletes specified object|objects\n" +
                "flags:\n" +
                "-all - delete all objects\n" +
                "-id=[value] - specify id of deleted object, value of said id must be signed integer," +
                "does not work with user (id is ignored and current user is deleted)";
        }

        public string GetName()
        {
            return "delete [flags] user|task|tag";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "delete")
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
