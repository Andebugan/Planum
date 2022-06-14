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

        public void Execute(string command)
        {
            string[] args = command.Split(' ');
            Log.Information("Create command was called");
            if (args[1] == "tag" && args.Length == 2)
            {
                CreateTag(args);
                return;
            }

            if (args[1] == "task" && args.Length == 2)
            {
                CreateTask(args);
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
            return;
        }

        public void CreateTag(string[] args)
        {
            Log.Information("Create tag command was called");

            if (args.Length == 2)
            {
                string? input;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("name: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Tag name can't be empty\n");
                    return;
                }
                string name = input;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("description: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                string? description = input;
                if (description == null)
                    description = "";

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("category: ");
                Console.ForegroundColor = ConsoleColor.White;

                string? category = Console.ReadLine();
                if (category == null)
                    category = "";

                _tagManager.CreateTag(category, name, description);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("successfully created new tag\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void CreateTask(string[] args)
        {
            Log.Information("clear task command was called");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter task name: ");
            Console.ForegroundColor = ConsoleColor.White;
            string? input = Console.ReadLine();

            string name;
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("task name can't be null or empty\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            name = input;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter task description: ");
            Console.ForegroundColor = ConsoleColor.White;
            string? description = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter tag ids: ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            List<int> tagIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                tagIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int tagId in tagIds)
            {
                if (_tagManager.FindTag(tagId) == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect tag id\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter parent ids: ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            List<int> parentIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                parentIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();
            foreach (int parentId in parentIds)
            {
                if (_taskManager.FindTask(parentId) == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect tag id\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter children ids: ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            List<int> childIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            foreach (int childId in childIds)
            {
                if (_taskManager.FindTask(childId) == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect tag id\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter status tags ids: ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            List<int> statusIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            foreach (int statusId in statusIds)
            {
                if (_tagManager.FindTag(statusId) == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect tag id\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("is task timed (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "n")
            {
                _taskManager.CreateTask(DateTime.Now, DateTime.Now, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("task created successfully\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter task start time as \"yyyy-mm-dd hh:mm\": ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            DateTime startTime;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("incorrect input\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter task deadline as \"yyyy-MM-dd HH:mm\": ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            DateTime deadline;
            if (string.IsNullOrEmpty(input))
                deadline = DateTime.MinValue;
            else if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("incorrect input\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("is task repeated (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "n")
            {
                _taskManager.CreateTask(startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                    description: description, timed: true);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("task created successfully\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Enter task repeat period as \"d:hh:mm\": ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine(); 
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input))
                repeatPeriod = TimeSpan.Zero;
            else if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.WriteLine();
            int newTaskId = _taskManager.CreateTask(startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name,
                description: description, timed: true, isRepeated: true);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("task created successfully\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "Creates specified object";
        }

        public string GetName()
        {
            return "create tag|task";
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
