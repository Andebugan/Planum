using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class AddCommand : ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public AddCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void AddChild()
        {
            Serilog.Log.Information("Add child to task command was called");
            string? input;
            Console.Write("Enter task id: ");
            input = Console.ReadLine();
            int taskId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(taskId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.Write("Enter child id: ");
            input = Console.ReadLine();
            int childId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out childId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(childId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.AddChildToTask(taskId, childId);
        }

        public void AddParent()
        {
            Serilog.Log.Information("Add parent to task command was called");
            string? input;
            Console.Write("Enter task id: ");
            input = Console.ReadLine();
            int taskId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(taskId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.Write("Enter parent id: ");
            input = Console.ReadLine();
            int parentId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out parentId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(parentId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.AddParentToTask(taskId, parentId);
        }

        public void AddTag()
        {
            Serilog.Log.Information("Add tag to task command was called");
            string? input;
            Console.Write("Enter task id: ");
            input = Console.ReadLine();
            int taskId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            if (_taskManager.FindTask(taskId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.Write("Enter tag id: ");
            input = Console.ReadLine();
            int tagId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out tagId))
            {
                Console.WriteLine("Tag id must be signed integer\n");
                return;
            }

            if (_tagManager.FindTag(tagId) == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }
            Console.WriteLine();
            _taskManager.AddTagToTask(taskId, tagId);
        }

        public void Execute(string command)
        {
            return;
        }

        public string GetDescription()
        {
            return "adds object/objects to the task";
        }

        public string GetName()
        {
            return "add -id=[value] tag|parent|child";
        }

        public bool IsAvaliable()
        {
            return true;
        }

        public bool IsCommand(string command)
        {
            if (command == "add")
                return true;
            return false;
        }
    }
}
