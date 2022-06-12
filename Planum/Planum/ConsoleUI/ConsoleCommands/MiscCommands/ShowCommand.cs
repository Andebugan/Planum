using Planum.ConsoleUI.ConsoleViews;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowCommand: ICommand
    {
        protected IUserManager _userManager;
        protected ITaskManager _taskManager;
        protected ITagManager _tagManager;

        public ShowCommand(IUserManager userManager, ITaskManager taskManager, ITagManager tagManager)
        {
            _userManager = userManager;
            _taskManager = taskManager;
            _tagManager = tagManager;
        }

        public void ShowCurrentUser()
        {
            Serilog.Log.Information("show current user command was called");
            if (_userManager.CurrentUser == null)
                return;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("id: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Id);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("login: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Login);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("password: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(_userManager.CurrentUser.Password);
            Console.WriteLine();
        }

        public void ShowAllUsers()
        {
            Serilog.Log.Information("show all users command was called");
            List<User> users = _userManager.GetAllUsers();
            if (users.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("there are no users in the system\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("existing users:\n");
            Console.ForegroundColor = ConsoleColor.White;
            foreach (User user in users)
            {
                Console.WriteLine(user.Login);
            }
            Console.WriteLine();
        }

        public void ShowTag(int id, bool showCategory = false, bool showDescription = false)
        {
            Serilog.Log.Information("show tag command was called");
            Tag? tag = _tagManager.FindTag(id);
            if (tag == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"tag with id={id} does not exist\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            TagListView tagListView = new TagListView();
            tagListView.RenderTag(tag, showCategory, showDescription);
        }

        public void ShowAllTags(bool showCategory = false, bool showDescription = false, List<string>? filters = null)
        {
            Serilog.Log.Information("show all tags command was called");
            List<Tag> tags = _tagManager.GetAllTags();

            if (tags.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("there are no tags in the system\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            bool parseSuccessfull = true;
            List<Tag> filteredTags = new List<Tag>();
            foreach (var filter in filters)
            {
                if (filter.Substring(0, 4) == "-cat")
                {
                    string category = filter.Substring(4);
                    foreach (var tag in tags)
                    {
                        if (tag.Category == category)
                        {
                            filteredTags.Add(tag);
                        }
                    }
                }
                else
                {
                    parseSuccessfull = false;
                    break;
                }
            }

            if (!parseSuccessfull)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("incorrect filter parameters\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            TagListView tagListView = new TagListView();
            tagListView.RenderTags(tags, showCategory, showDescription);
        }

        public void ShowTask()
        {
            Serilog.Log.Information("Show task command was called");
            Console.Write("Enter task id: ");
            int taskId;
            if (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            Task? task = _taskManager.FindTask(taskId);

            if (task == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.WriteLine("Task id: " + task.Id);
            Console.WriteLine("Task name: " + task.Name);
            Console.WriteLine("Task description: " + task.Description);
            Console.Write("Task tags: ");
            foreach (int id in task.TagIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task current status: " + task.StatusQueueIds[task.CurrentStatusIndex]);
            Console.Write("Task statuses: ");
            foreach (int id in task.StatusQueueIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task parents: ");
            foreach (int id in task.ParentIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task children: ");
            foreach (int id in task.ChildIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task is timed: " + task.Timed);
            Console.WriteLine("Task start time: " + task.StartTime.ToString());
            Console.WriteLine("Task deadline: " + task.Deadline.ToString());
            Console.WriteLine("Task is repeated: " + task.IsRepeated);
            Console.WriteLine("Task repeat period: " + task.RepeatPeriod.ToString());
            Console.WriteLine();
        }

        public void ShowAllTasks()
        {
            Serilog.Log.Information("Show all tasks command was called");
            List<Task> tasks = _taskManager.GetAllTasks();
            foreach (Task task in tasks)
            {
                Console.WriteLine("Task id: " + task.Id);
                Console.WriteLine("Task name: " + task.Name);
                Console.WriteLine("Task description: " + task.Description);
                Console.Write("Task tags: ");
                foreach (int id in task.TagIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task current status: " + task.StatusQueueIds[task.CurrentStatusIndex]);
                Console.Write("Task statuses: ");
                foreach (int id in task.StatusQueueIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task parents: ");
                foreach (int id in task.ParentIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task children: ");
                foreach (int id in task.ChildIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task is timed: " + task.Timed);
                Console.WriteLine("Task start time: " + task.StartTime.ToString());
                Console.WriteLine("Task deadline: " + task.Deadline.ToString());
                Console.WriteLine("Task is repeated: " + task.IsRepeated);
                Console.WriteLine("Task repeat period: " + task.RepeatPeriod.ToString());
                Console.WriteLine();
            }
        }

        public void ShowArchivedTasks()
        {
            Serilog.Log.Information("Show archived task command was called");
            Console.Write("Enter task id: ");
            int taskId;
            if (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Task id must be signed integer\n");
                return;
            }

            Task? archivedTask = _taskManager.FindTask(taskId, true);

            if (archivedTask == null)
            {
                Console.WriteLine("Task with specified id does not exist\n");
                return;
            }

            Console.WriteLine("Task id: " + archivedTask.Id);
            Console.WriteLine("Task name: " + archivedTask.Name);
            Console.WriteLine("Task description: " + archivedTask.Description);
            Console.Write("Task tags: ");
            foreach (int id in archivedTask.TagIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task current status: " + archivedTask.StatusQueueIds[archivedTask.CurrentStatusIndex]);
            Console.Write("Task statuses: ");
            foreach (int id in archivedTask.StatusQueueIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task parents: ");
            foreach (int id in archivedTask.ParentIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task children: ");
            foreach (int id in archivedTask.ChildIds)
                Console.Write(id + " ");
            Console.WriteLine();
            Console.WriteLine("Task is timed: " + archivedTask.Timed);
            Console.WriteLine("Task start time: " + archivedTask.StartTime.ToString());
            Console.WriteLine("Task deadline: " + archivedTask.Deadline.ToString());
            Console.WriteLine("Task is repeated: " + archivedTask.IsRepeated);
            Console.WriteLine("Task repeat period: " + archivedTask.RepeatPeriod.ToString());
            Console.WriteLine();
        }

        public void ShowAllArchived()
        {
            Serilog.Log.Information("Show all archived tasks command was called");
            List<Task> archivedTasks = _taskManager.GetAllTasks(true);
            foreach (Task archivedTask in archivedTasks)
            {
                Console.WriteLine("Task id: " + archivedTask.Id);
                Console.WriteLine("Task name: " + archivedTask.Name);
                Console.WriteLine("Task description: " + archivedTask.Description);
                Console.Write("Task tags: ");
                foreach (int id in archivedTask.TagIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task parents: ");
                foreach (int id in archivedTask.ParentIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task children: ");
                foreach (int id in archivedTask.ChildIds)
                    Console.Write(id + " ");
                Console.WriteLine();
                Console.WriteLine("Task is timed: " + archivedTask.Timed);
                Console.WriteLine("Task start time: " + archivedTask.StartTime.ToString());
                Console.WriteLine("Task deadline: " + archivedTask.Deadline.ToString());
                Console.WriteLine("Task is repeated: " + archivedTask.IsRepeated);
                Console.WriteLine("Task repeat period: " + archivedTask.RepeatPeriod.ToString());
                Console.WriteLine();
            }
        }

        public void Execute(string command)
        {
            string[] args = command.Split();

            if (args[args.Length - 1] == "user")
            {
                if (args.Length == 2 && _userManager.CurrentUser == null)
                {
                    ShowAllUsers();
                    return;
                }

                if (args.Length == 2 && _userManager.CurrentUser != null)
                {
                    ShowCurrentUser();
                    return;
                }
            }

            if (args[args.Length - 1] == "tag")
            {
                bool parseSuccessfull = true;
                bool showDescription = false;
                bool showCategory = false;
                bool showAll = true;

                List<string> filters = new List<string>();
                int id = -1;

                List<string> argsList = new List<string>(args);
                argsList.Remove("show");
                argsList.Remove("tag");

                for (int i = 0; i < argsList.Count; i++)
                {
                    if (argsList[i] == "-c" && !showCategory)
                        showCategory = true;
                    else if (argsList[i] == "-d" && !showDescription)
                        showDescription = true;
                    else if (argsList[i].Substring(0, 4) == "-id=" && showAll)
                    {
                        if (!int.TryParse(argsList[i].Substring(4), out id) || id < 0)
                        {
                            parseSuccessfull = false;
                            break;
                        }
                        showAll = false;
                    }
                    else if (argsList[i].Substring(0, 2) == "-s")
                    {
                        if (argsList[i].Substring(0, 7) == "-s-cat=")
                        {
                            string category = argsList[i].Substring(7);
                            i += 1;
                            while (argsList[i][0] != '-' && i < argsList.Count)
                            {
                                category += " " + argsList[i];
                                i += 1;
                            }

                            category = category.Replace("\"", "");
                            filters.Add("-cat" + category);

                            if (i == argsList.Count)
                                break;
                            else
                                i -= 1;
                        }
                    }
                    else
                    {
                        parseSuccessfull = false;
                        break;
                    }
                }

                if (parseSuccessfull)
                {
                    if (showAll)
                    {
                        ShowAllTags(showCategory, showDescription, filters);
                        return;
                    }
                    else if (filters.Count == 0)
                    {
                        ShowTag(id, showCategory, showDescription);
                        return;
                    }
                }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Incorrect command parameters\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            if (_userManager.CurrentUser == null)
                return "displays objects, shows all existing by default";
            else
                return "displays objects, shows all existing by default\n" +
                    "flags:\n" +
                    "-id={value} - specify id of displayed object, value of said id must be signed integer\n" +
                    "tag:\n" +
                    "-c - show category\n" +
                    "-d - show description\n" +
                    "-s - sort by:\n" +
                        "-cat={value} - sort by category (-s-cat=\"category 1\")\n" +
                    "task:\n" +
                    "-archived - show archived tasks\n" + 
                    "-l [options] - display tasks list with list options";
        }

        public string GetName()
        {
            if (_userManager.CurrentUser == null)
                return "show user";
            else
                return "show [-l] [-id={value}] [-archived] task\n" +
                    "show [-id={value}] [-c] [-d] tag\n" +
                    "show [-c] [-d] [-s[-cat={value}]] tag\n" +
                    "show user";
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "show")
                return true;
            return false;
        }

        public bool IsAvaliable()
        {
            return true;
        }
    }
}
