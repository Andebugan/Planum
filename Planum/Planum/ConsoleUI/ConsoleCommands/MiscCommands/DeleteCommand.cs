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
            Console.WriteLine("tag was successfully deleted\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DeleteAllTags()
        {
            Log.Information("delete all tags command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("confirm deleting all tags (y/n): ");
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
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("canceled deletion of all tags\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DeleteTask(int id)
        {
            Log.Information("delete task command was called");
            _taskManager.DeleteTask(id);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("task was successfully deleted\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DeleteAllTasks()
        {
            Log.Information("delete all tasks command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("confirm deleting all tasks (y/n): ");
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
                List<Task> tasks = _taskManager.GetAllTasks(null);
                foreach (Task task in tasks)
                    _taskManager.DeleteTask(task.Id);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("successfully deleted all tasks\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("canceled deletion of all tasks\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Execute(string command)
        {
            string[] args = command.Split(' ');
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
                    return;
                }
            }

            if (args[args.Length - 1] == "task")
            {
                List<string> argsList = new List<string>(args);
                argsList.Remove("delete");
                argsList.Remove("task");

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
                        DeleteAllTasks();
                    else
                        DeleteTask(id);
                    return;
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
