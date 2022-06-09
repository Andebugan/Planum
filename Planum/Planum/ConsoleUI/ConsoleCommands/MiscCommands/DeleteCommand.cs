using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
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

        public void DeleteTag()
        {
            Serilog.Log.Information("Delete tag command was called");
            Console.Write("Deleted task id: ");
            string? input = Console.ReadLine();
            int id;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer\n");
                return;
            }
            _tagManager.DeleteTag(id);
            Console.WriteLine();
        }

        public void DeleteAllTags()
        {
            Log.Information("Delete all tags command was called");
            List<Tag> tags = _tagManager.GetAllTags();
            foreach (Tag tag in tags)
                _tagManager.DeleteTag(tag.Id);
            Console.WriteLine();
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
