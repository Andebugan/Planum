using Alba.CsConsoleFormat;
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
                if (string.IsNullOrEmpty(login) || string.IsNullOrWhiteSpace(login))
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

                if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
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
                        if (checkPassword != null && checkPassword.Length > 0)
                            checkPassword = checkPassword.Substring(0, checkPassword.Length - 1);
                    }
                    else
                        checkPassword += key.KeyChar;
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
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
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
            Console.WriteLine("successfully updated tag\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void UpdateTask(int id)
        {
            Log.Information("update task command was called");

            Task? task = _taskManager.FindTask(id);
            if (task == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("task with specified id does not exists\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            string? input;

            // name
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("name: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(task.Name);
            string? name = task.Name;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change name (y/n): ");
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
                name = "";
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("new name: ");
                Console.ForegroundColor = ConsoleColor.White;
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("name can't be null or empty\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            // description
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("description: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(task.Description);
            string? description = task.Description;

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
                {
                    description = "";
                }
            }

            // tag ids
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("tags:\n");
            Console.ForegroundColor = ConsoleColor.White;
            var doc = new Document();

            Grid grid = new Grid();
            grid.Color = ConsoleColor.DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(GridLength.Auto);

            foreach (var tag in task.TagIds)
            {
                grid.Children.Add(new Cell(_tagManager.GetTag(tag).Id) { Align = Align.Left, Color = ConsoleColor.White });
                grid.Children.Add(new Cell(_tagManager.GetTag(tag).Name) { Align = Align.Left, Color = ConsoleColor.White });
            }

            doc.Children.Add(grid);
            ConsoleRenderer.RenderDocument(doc);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change tags (y/n): ");
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

            List<int> tagIds = (List<int>)task.TagIds;

            if (input == "y")
            {
                tagIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new tag ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
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
            }

            // parent ids
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("parents:\n");
            Console.ForegroundColor = ConsoleColor.White;
            doc = new Document();

            grid = new Grid();
            grid.Color = ConsoleColor.DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(GridLength.Auto);

            foreach (var parent in task.ParentIds)
            {
                grid.Children.Add(new Cell(_taskManager.GetTask(parent).Id) { Align = Align.Left, Color = ConsoleColor.White });
                grid.Children.Add(new Cell(_taskManager.GetTask(parent).Name) { Align = Align.Left, Color = ConsoleColor.White });
            }

            doc.Children.Add(grid);
            ConsoleRenderer.RenderDocument(doc);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change parents (y/n): ");
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

            List<int> parentIds = (List<int>)task.ParentIds;

            if (input == "y")
            {
                parentIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new parent ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
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
                    if (_tagManager.FindTag(parentId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect parent id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            // child ids
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("children:\n");
            Console.ForegroundColor = ConsoleColor.White;
            doc = new Document();

            grid = new Grid();
            grid.Color = ConsoleColor.DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(GridLength.Auto);

            foreach (var children in task.ChildIds)
            {
                grid.Children.Add(new Cell(_taskManager.GetTask(children).Id) { Align = Align.Left, Color = ConsoleColor.White });
                grid.Children.Add(new Cell(_taskManager.GetTask(children).Name) { Align = Align.Left, Color = ConsoleColor.White });
            }

            doc.Children.Add(grid);
            ConsoleRenderer.RenderDocument(doc);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change children (y/n): ");
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

            List<int> childIds = (List<int>)task.ChildIds;

            if (input == "y")
            {
                parentIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter new child ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
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
                    if (_tagManager.FindTag(childId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect child id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            // status ids
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("status queue:\n");
            Console.ForegroundColor = ConsoleColor.White;
            doc = new Document();

            grid = new Grid();
            grid.Color = ConsoleColor.DarkGray;

            grid.Columns.Add(GridLength.Auto);
            grid.Columns.Add(GridLength.Auto);

            foreach (var status in task.StatusQueueIds)
            {
                grid.Children.Add(new Cell(_tagManager.GetTag(status).Id) { Align = Align.Left, Color = ConsoleColor.White });
                grid.Children.Add(new Cell(_tagManager.GetTag(status).Name) { Align = Align.Left, Color = ConsoleColor.White });
            }

            doc.Children.Add(grid);
            ConsoleRenderer.RenderDocument(doc);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change statuses (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            List<int> statusQueueIds = (List<int>)task.StatusQueueIds;

            if (input == "y")
            {
                statusQueueIds = new List<int>();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter status ids: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
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
                        statusQueueIds.Add(tempId);
                    }
                }

                foreach (int statusId in statusQueueIds)
                {
                    if (_tagManager.FindTag(statusId) == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("incorrect status id\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
            }

            // current status id
            int currentStatusIndex = 0;
            if (statusQueueIds.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter current status index: ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (int.TryParse(input, out currentStatusIndex))
                {
                    if (currentStatusIndex < 0 || currentStatusIndex >= statusQueueIds.Count)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("current status index must be in range from 0 to status queue length\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            // timed
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("timed (y/n): ");
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
                Task newTask = new Task(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                    tagIds, parentIds, childIds, name, false,
                    task.UserId, description, StatusQueueIds: statusQueueIds);
                newTask.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("succsessfully updated task\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change time parameters (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("should have entered y or x\n\n");
                return;
            }

            if (input == "n")
            {
                Task newTask = new Task(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod,
                    tagIds, parentIds, childIds, name, task.Timed,
                    task.UserId, description, StatusQueueIds: statusQueueIds);
                newTask.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("succsessfully updated task\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            // start time
            if (task.StartTime == DateTime.MinValue)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("start time not set");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("start time: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.StartTime.Year.ToString() + "-" +
                            task.StartTime.Month.ToString() + "-" +
                            task.StartTime.Day.ToString() + " " +
                            task.StartTime.Hour.ToString() + ":" +
                            task.StartTime.Minute.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change start time (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            DateTime startTime = task.StartTime;

            if (input == "y")
            {

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter task start time as yyyy-mm-dd hh:mm : ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            // deadline
            if (task.Deadline == DateTime.MinValue)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("deadline not set\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("deadline: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(task.Deadline.Year.ToString() + "-" +
                            task.Deadline.Month.ToString() + "-" +
                            task.Deadline.Day.ToString() + " " +
                            task.Deadline.Hour.ToString() + ":" +
                            task.Deadline.Minute.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change deadline (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            DateTime deadline = task.Deadline;

            if (input == "y")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter task deadline as yyyy-mm-dd hh:mm : ");
                Console.ForegroundColor = ConsoleColor.White;
                input = Console.ReadLine();
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out deadline))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            // isRepeated
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("repeated (y/n): ");
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
                if (startTime > deadline)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("start time can't be after deadline\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }

                Task newTask = new Task(task.Id, startTime, deadline, TimeSpan.Zero, tagIds, parentIds, childIds, name, true,
                    task.UserId, description, false, task.Archived, statusQueueIds);
                newTask.SetStatusIndex(currentStatusIndex);
                _taskManager.UpdateTask(newTask);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("succsessfully updated task\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            // repeat period
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("repeat period: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(task.RepeatPeriod.Days.ToString() + "d "
                + task.RepeatPeriod.Hours.ToString() + "h "
                + task.RepeatPeriod.Minutes.ToString() + "m");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("change repeat period (y/n): ");
            Console.ForegroundColor = ConsoleColor.White;
            input = Console.ReadLine();
            if (input != "y" && input != "n")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("should have entered y or x\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            TimeSpan repeatPeriod = task.RepeatPeriod;

            if (input == "y")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("enter task repeat period as yyyy-mm-dd hh:mm : ");
                Console.ForegroundColor = ConsoleColor.White;

                input = Console.ReadLine();
                if (!TimeSpan.TryParseExact(input, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, TimeSpanStyles.None, out repeatPeriod))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("incorrect input\n");
                    Console.ForegroundColor = ConsoleColor.White;
                    return;
                }
            }

            Task newTask_ = new Task(task.Id, startTime, deadline, repeatPeriod, tagIds, parentIds, childIds, name, true,
                        task.UserId, description, true, task.Archived, statusQueueIds);
            newTask_.SetStatusIndex(currentStatusIndex);
            _taskManager.UpdateTask(newTask_);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("succsessfully updated task\n");
            Console.ForegroundColor = ConsoleColor.White;
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
