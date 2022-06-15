using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
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

        public void UpdateUser(int id)
        {
            Log.Information("update user command was called");

            User? user = _userManager.FindUser(id);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"user with id={id}" +
                    $" does not exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("login: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Login);
            string? login = _userManager.CurrentUser.Login;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change login (y/n): ");
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
                login = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new login: ");
                Console.ForegroundColor = ConsoleColor.White;
                login = Console.ReadLine();
                if (string.IsNullOrEmpty(login))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("login can't be empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            if (_userManager.FindUser(login) != null && login != _userManager.CurrentUser.Login)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("user with this login already exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("password: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Password);

            string? password = _userManager.CurrentUser.Password;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change login (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = null;
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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new password: ");
                Console.ForegroundColor = ConsoleColor.White;
                password = "";
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (password != null && password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                    }
                    else
                        password += key.KeyChar;
                }

                if (string.IsNullOrEmpty(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("password can't be null\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("repeat password: ");
                Console.ForegroundColor = ConsoleColor.White;

                string? checkPassword = null;
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                        break;
                    else if (key.Key == ConsoleKey.Backspace)
                    {
                        if (password != null && password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                    }
                    else
                        password += key.KeyChar;
                }
                Console.WriteLine();

                if (password != checkPassword)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("passwords do not match\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            _userManager.UpdateUser(new User(user.Id, login, password));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("user updated successfully\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateTag(int id)
        {
            Log.Information("update tag command was called");

            Tag? tag = _tagManager.FindTag(id);
            if (tag == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"tag with id={id} does not exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("name: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(tag.Name);
            string? name = tag.Name;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change name (y/n): ");
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
                name = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new name: ");
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("name can't be empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("category: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(tag.Category);
            string? category = tag.Category;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change category (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = null;
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
                category = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new category: ");
                Console.ForegroundColor = ConsoleColor.White;
                category = Console.ReadLine();
                if (category == null)
                    category = "";
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("description: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(tag.Description);
            string? description = tag.Description;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change description (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = null;
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
                description = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new description: ");
                Console.ForegroundColor = ConsoleColor.White;
                description = Console.ReadLine();
                if (description == null)
                    description = "";
            }

            _tagManager.UpdateTag(new Tag(tag.Id, tag.UserId, category, name, description));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("tag updated successfully\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateTask(int id)
        {
            Log.Information("update task command was called");
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

        public void Execute(string command)
        {
            string[] args = command.Split(' ');
            if (args.Length == 2 && args[1] == "user")
            {
                UpdateUser(_userManager.CurrentUser.Id);
                return;
            }

            if (args[args.Length - 1] == "tag" && args.Length == 3)
            {
                if (args[1].Substring(0, 4) == "-id=")
                {
                    int id = -1;
                    if (int.TryParse(args[1].Substring(4), out id) && id >= 0)
                    {
                        UpdateTag(id);
                        return;
                    }
                }
            }

            if (args[args.Length - 1] == "task" && args.Length == 3)
            {
                if (args[1].Substring(0, 4) == "-id=")
                {
                    int id = -1;
                    if (int.TryParse(args[1].Substring(4), out id) && id >= 0)
                    {
                        UpdateTask(id);
                        return;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "updates specified object|objects";
        }

        public string GetName()
        {
            return "update -id={value} task\n" +
                "update -id={value} tag\n" +
                "update user";
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
