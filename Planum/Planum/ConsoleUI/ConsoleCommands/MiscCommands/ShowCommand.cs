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
            Serilog.Log.Information("Show current user command was called");
            if (_userManager.CurrentUser == null)
                return;
            Console.WriteLine("id: " + _userManager.CurrentUser.Id);
            Console.WriteLine("login: " + _userManager.CurrentUser.Login);
            Console.WriteLine("password: " + _userManager.CurrentUser.Password);
            Console.WriteLine();
        }

        public void ShowAllUsers()
        {
            Serilog.Log.Information("Show all users command was called");
            List<User> users = _userManager.GetAllUsers();
            foreach (User user in users)
            {
                Console.WriteLine("login: " + user.Login);
            }
            Console.WriteLine();
        }

        public void ShowTag()
        {
            Serilog.Log.Information("Show tag command was called");
            int id = 0;
            Console.Write("Enter id: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || !int.TryParse(input, out id))
            {
                Console.WriteLine("Id must be signed integer\n");
                return;
            }
            Tag? tag = _tagManager.FindTag(id);

            Console.WriteLine("Tag id: " + tag.Id);
            Console.WriteLine("Tag name: " + tag.Name);
            Console.WriteLine("Tag description: " + tag.Description);
            Console.WriteLine("Tag category: " + tag.Category);
            Console.WriteLine();
        }

        public void ShowAllTags()
        {
            Serilog.Log.Information("Show all tags command was called");
            List<Tag> tags = _tagManager.GetAllTags();
            foreach (Tag tag in tags)
            {
                Console.WriteLine("Tag id: " + tag.Id);
                Console.WriteLine("Tag name: " + tag.Name);
                Console.WriteLine("Tag description: " + tag.Description);
                Console.WriteLine("Tag category: " + tag.Category);
                Console.WriteLine();
            }
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

        public void Execute(string[] args)
        {
            // when show check user
            return;
        }

        public string GetDescription()
        {
            return "displays specified object|objects\n" +
                "flags:\n" +
                "-all - display all objects, used by default\n" +
                "for tag and task:\n" +
                "-l [options] - display tasks or tags as a list with list options\n" +
                "-id=[value] - specify id of displayed object, value of said id must be signed integer";
        }

        public string GetName()
        {
            return "show [options] user|task|tag";
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
