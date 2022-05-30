using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class CreateTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public CreateTaskCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
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

            Console.Write("Enter parent ids: ");
            input = Console.ReadLine();
            List<int> parentIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                parentIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            Console.Write("Enter children ids: ");
            input = Console.ReadLine();
            List<int> childIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

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
            _taskManager.CreateTask(startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name,
                description: description, timed: true, isRepeated : true);
        }

        public string GetDescription()
        {
            return "creates task with specified params";
        }

        public string GetName()
        {
            return "create task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
