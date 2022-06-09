using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class UpdateCommand: ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public UpdateCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void UpdateUser()
        {
            Serilog.Log.Information("Update user command was called");
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            int id;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("id must be signed integer\n");
                return;
            }

            User? user = _userManager.FindUser(id);
            if (user == null)
            {
                Console.WriteLine("User with entered id does not exist\n");
                return;
            }

            Console.Write("Enter new login: ");
            string? login = Console.ReadLine();
            if (string.IsNullOrEmpty(login))
            {
                Console.WriteLine("login can't be null or empty\n");
                return;
            }
            Console.Write("Enter new password: ");
            string? password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("password can't be null\n");
                return;
            }
            _userManager.UpdateUser(new User(user.Id, login, password));
            Console.WriteLine();
        }

        public void UpdateTag()
        {
            Serilog.Log.Information("Update tag command was called");
            int id = 0;
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer\n");
                return;
            }

            Tag? tag = _tagManager.FindTag(id);
            if (tag == null)
            {
                Console.WriteLine("User with specified id does not exist\n");
                return;
            }

            Console.Write("Enter name: ");
            string? name = Console.ReadLine();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Name can't be null or zero\n");
                return;
            }

            Console.Write("Enter descriptions: ");
            string? description = Console.ReadLine();
            if (description == null)
                description = "";

            Console.Write("Enter category: ");
            input = Console.ReadLine();
            int category;
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out category))
            {
                Console.WriteLine("Category must be signed integer\n");
                return;
            }
            Console.WriteLine();

            _tagManager.UpdateTag(new Tag(tag.Id, tag.UserId, category, name, description));
        }

        public void UpdateTask()
        {
            Serilog.Log.Information("Update task command was called");
            Console.Write("Enter task id: ");
            int id;
            if (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            Task? task = _taskManager.FindTask(id);
            if (task == null)
            {
                Console.WriteLine("Task with specified id does not exists\n");
                return;
            }

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

            Console.Write("Enter status ids: ");
            input = Console.ReadLine();
            List<int> statusQueueIds = new List<int>();
            if (!string.IsNullOrEmpty(input))
                childIds = input.Split(' ').Select(n => Convert.ToInt32(n)).ToList<int>();

            int currentStatusIndex = 0;

            Console.Write("Is task timed (y/n): ");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Must be y of n\n");
                return;
            }

            if (input == "n")
            {
                Task newTask = new Task(task.Id, DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero, tagIds, parentIds, childIds, name, task.Timed,
                    task.UserId, description, StatusQueueIds: statusQueueIds);
                newTask.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask);
                return;
            }

            Console.Write("Enter task start time as \"yyyy-mm-dd hh:mm\": ");
            input = Console.ReadLine();
            DateTime startTime;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                Console.WriteLine("Incorrect input\n");
                return;
            }

            Console.Write("Enter task deadline as \"yyyy-mm-dd hh:mm\": ");
            input = Console.ReadLine();
            DateTime deadline;
            if (string.IsNullOrEmpty(input))
                deadline = DateTime.MinValue;
            if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
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
                Task newTask = new Task(task.Id, startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name, task.Timed,
                    task.UserId, description, task.IsRepeated, task.Archived, statusQueueIds);
                newTask.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask);
                return;
            }

            Console.Write("Enter task repeat period as \"yyyy-mm-dd hh:mm\": ");
            input = Console.ReadLine();
            TimeSpan repeatPeriod;
            if (string.IsNullOrEmpty(input))
                startTime = DateTime.MinValue;
            if (!TimeSpan.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
            {
                Console.WriteLine("Incorrect input\n");
                return;
            }
            Console.WriteLine();
            Task newTask_ = new Task(task.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name, task.Timed,
                    task.UserId, description, task.IsRepeated, task.Archived, statusQueueIds);
            newTask_.SetStatusIndex(currentStatusIndex);
            _taskManager.UpdateTask(newTask_);
        }

        public void Execute(string[] args)
        {
            return;
        }

        public string GetDescription()
        {
            return "updates specified object|objects";
        }

        public string GetName()
        {
            return "update user|task|tag";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "update")
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
