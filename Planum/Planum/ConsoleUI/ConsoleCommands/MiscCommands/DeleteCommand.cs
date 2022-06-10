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
            Log.Information("delete user command was called");
            _userManager.DeleteUser(_taskManager, _tagManager);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("user was successfully deleted\n\n");
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

        public void DeleteAllTasks()
        {
            Serilog.Log.Information("Delete all tasks command was called");
            _taskManager.GetAllTasks(null).ForEach(task => _taskManager.DeleteTask(task.Id));
        }

        public void DeleteTask()
        {
            Serilog.Log.Information("Delete task command was called");
            Console.Write("Enter task id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(id) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            _taskManager.DeleteTask(id);
        }

        public void Execute(string[] args)
        {
            if (args.Length == 2 && args[1] == "user")
            {
                DeleteUser();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "deletes specified object|objects, deletes all by default\n" +
                "flags:\n" + 
                "-id=[value] - specify id of deleted object, value of said id must be signed integer";
        }

        public string GetName()
        {
            return "delete {[-id={value} task|tag}|user";
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
