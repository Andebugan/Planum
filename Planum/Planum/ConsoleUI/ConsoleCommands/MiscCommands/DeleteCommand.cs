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

        public void DeleteTag(int id)
        {
            Log.Information("delete tag command was called");
            _tagManager.DeleteTag(id);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("tag was successfully deleted\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DeleteAllTags()
        {
            Log.Information("delete all tags command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("confirm deleting all tags (y/n): \n");
            Console.ForegroundColor = ConsoleColor.White;

            string? input = null;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "y")
            {
                List<Tag> tags = _tagManager.GetAllTags();
                foreach (Tag tag in tags)
                    _tagManager.DeleteTag(tag.Id);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("successfully deleted all tags\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("canceled deletion of all tags\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DeleteAllTasks()
        {
            Log.Information("Delete all tasks command was called");
            _taskManager.GetAllTasks(null).ForEach(task => _taskManager.DeleteTask(task.Id));
        }

        public void DeleteTask()
        {
            Log.Information("Delete task command was called");
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

            if (args[args.Length - 1] == "tag")
            {
                List<string> argsList = new List<string>(args);
                argsList.Remove("delete");
                argsList.Remove("tag");

                bool parseSuccessfull = true;
                bool deleteAll = true;
                int id = -1;

                foreach (var arg in argsList)
                {
                    if (arg.Substring(0, 4) == "-id=" && deleteAll)
                    {
                        if (!int.TryParse(arg.Substring(4), out id) || id < 0)
                        {
                            parseSuccessfull = false;
                            break;
                        }
                        deleteAll = false;
                    }
                    else
                    {
                        parseSuccessfull = false;
                        break;
                    }
                }

                if (parseSuccessfull)
                {
                    if (deleteAll)
                        DeleteAllTags();
                    else
                        DeleteTag(id);
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
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
            return "delete [-id={value}] task|tag\n" +
                "delete user";
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
