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
                if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
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

                int id = _tagManager.CreateTag(category, name, description);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"new tag with id={id} created successfully\n");
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
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
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
            {
                string[] temp = input.Split(' ');
                foreach (string s in temp)
                {
                    int tempId;
                    if (!int.TryParse(s, out tempId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    tagIds.Add(tempId);
                }
            }
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
            {
                string[] temp = input.Split(' ');
                foreach (string s in temp)
                {
                    int tempId;
                    if (!int.TryParse(s, out tempId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    parentIds.Add(tempId);
                }
            }
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
            {
                string[] temp = input.Split(' ');
                foreach (string s in temp)
                {
                    int tempId;
                    if (!int.TryParse(s, out tempId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    childIds.Add(tempId);
                }
            }

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
            {
                string[] temp = input.Split(' ');
                foreach (string s in temp)
                {
                    int tempId;
                    if (!int.TryParse(s, out tempId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect input\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                    statusIds.Add(tempId);
                }
            }

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
            if (string.IsNullOrEmpty(input) && input != "x" && input != "y")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "n")
            {
                int id = _taskManager.CreateTask(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                false, description, false, statusIds);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"new task with id={id} created successfully\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter task start time as yyyy-mm-dd hh:mm (leave empty for default value): ");
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
            Console.Write("enter task deadline as yyyy-MM-dd hh:mm (leave empty for default value): ");
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
            if (string.IsNullOrEmpty(input) && input != "x" && input != "y")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            if (input == "n")
            {
                if (startTime > deadline)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("start time can't be after deadline\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                int id = _taskManager.CreateTask(startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name,
                true, description, false, statusIds);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"task with id={id} created successfully\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("enter task repeat period as d:hh:mm : ");
            Console.ForegroundColor = ConsoleColor.White;

            input = Console.ReadLine(); 
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input) && input != "x" && input != "y")
                repeatPeriod = TimeSpan.Zero;
            else if (!TimeSpan.TryParseExact(input, @"d\:hh\:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            int id_ = _taskManager.CreateTask(startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name,
                true, description, true, statusIds);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"task with id={id_} created successfully\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "creates specified object";
        }

        public string GetName()
        {
            return "create tag\n" +
                "create tag";
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
