using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class RemoveCommand : ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public RemoveCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void RemoveParent()
        {
            Serilog.Log.Information("Remove parent from task command was called");
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

            Console.WriteLine("Enter parent id: ");
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
            _taskManager.RemoveParentFromTask(taskId, parentId);
        }

        public void RemoveChild()
        {
            Serilog.Log.Information("Remove child from task command was called");
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

            Console.WriteLine("Enter child id: ");
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
            _taskManager.RemoveChildFromTask(taskId, childId);
        }

        public void RemoveTag()
        {
            Serilog.Log.Information("Remove tag from task command was called");
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

            Console.WriteLine("Enter tag id: ");
            input = Console.ReadLine();
            int tagId;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out tagId))
            {
                Console.WriteLine("Tag id must be signed integer\n");
                return;
            }
            Console.WriteLine();
            _taskManager.RemoveTagFromTask(taskId, tagId);
        }

        public void Execute(string[] args)
        {
            return;
        }

        public string GetDescription()
        {
            return "adds object/objects to the task";
        }

        public string GetName()
        {
            return "exit";
        }

        public bool IsAvaliable()
        {
            return true;
        }

        public bool IsCommand(string command)
        {
            if (command == "remove")
                return true;
            return false;
        }
    }
}
