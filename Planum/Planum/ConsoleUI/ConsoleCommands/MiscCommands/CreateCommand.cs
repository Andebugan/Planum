using Planum.Models.BuisnessLogic.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class CreateCommand: ICommand
    {
        ITagManager _tagManager;
        IUserManager _userManager;
        ITaskManager _taskManager;

        public CreateCommand(ITagManager tagManager, IUserManager userManager, ITaskManager taskManager)
        {
            _tagManager = tagManager;
            _userManager = userManager;
            _taskManager = taskManager;
        }

        public void Execute(string[] args)
        {
            Log.Information("Create command was called");
            if (args.Length == 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("object unspecified");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void CreateTag(string[] args)
        {
            Log.Information("Create tag command was called");
            string? input = null;
            Console.Write("Enter tag name: ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Tag name can't be empty\n");
                return;
            }
            string name = input;

            Console.Write("Enter tag description: ");
            input = Console.ReadLine();
            string? description = input;
            if (description == null)
                description = "";

            Console.Write("Enter tag category: ");
            input = Console.ReadLine();
            int category;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out category))
            {
                Console.WriteLine("Tag category must be signed integer\n");
                return;
            }
            Console.WriteLine();
            _tagManager.CreateTag(category, name, description);
        }

        public void CreateTask(string[] args)
        {
            Log.Information("Clear task command was called");
            Console.Write("Enter task name: ");
            string? input = Console.ReadLine();
            string name;
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Task name can't be null or empty\n");
                return;
            }
            name = input;

            Console.Write("Enter task description: ");
            string? description = Console.ReadLine();

            Console.Write("Enter tag ids: ");
            input = Console.ReadLine();
            List<int> tagIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                tagIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int tagId in tagIds)
            {
                if (_tagManager.FindTag(tagId) == null)
                    return;
            }

            Console.Write("Enter parent ids: ");
            input = Console.ReadLine();
            List<int> parentIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                parentIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int parentId in parentIds)
            {
                if (_taskManager.FindTask(parentId) == null)
                    return;
            }

            Console.Write("Enter children ids: ");
            input = Console.ReadLine();
            List<int> childIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            foreach (int childId in childIds)
            {
                if (_taskManager.FindTask(childId) == null)
                    return;
            }

            Console.Write("Is task timed (y/n): ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Must be y of n\n");
                return;
            }

            if (input == "n")
            {
                _taskManager.CreateTask(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description);
                return;
            }

            Console.Write("Enter task start time as \"yyyy-mm-dd hh:mm\": ");
            input = Console.ReadLine();
            DateTime startTime;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                Console.WriteLine("Incorrect input\n");
                return;
            }

            Console.Write("Enter task deadline as \"yyyy-MM-dd HH:mm\": ");
            input = Console.ReadLine();
            DateTime deadline;
            if (string.IsNullOrEmpty(input))
                deadline = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
            {
                Console.WriteLine("Incorrect input\n");
                return;
            }

            Console.Write("Is task repeated (y/n): ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Must be y of n\n");
                return;
            }

            if (input == "n")
            {
                _taskManager.CreateTask(startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description, timed: true);
                return;
            }

            Console.Write("Enter task repeat period as \"d:hh:mm\": ");
            input = Console.ReadLine();
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input))
                repeatPeriod = TimeSpan.Zero;
            else if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                Console.WriteLine("Incorrect input\n");
                return;
            }
            Console.WriteLine();
            int newTaskId = _taskManager.CreateTask(startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name,
                description: description, timed: true, isRepeated: true);

            foreach (int taskId in parentIds)
            {
                _taskManager.AddChildToTask(taskId, newTaskId);
            }

            foreach (int taskId in childIds)
            {
                _taskManager.AddParentToTask(taskId, newTaskId);
            }
        }

        public string GetDescription()
        {
            return "Creates specified object\n" +
                "flags:\n" +
                "-d - create with default values";
        }

        public string GetName()
        {
            return "create [flags] task|tag";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "create")
                return true;
            return false;
        }
    }
}
