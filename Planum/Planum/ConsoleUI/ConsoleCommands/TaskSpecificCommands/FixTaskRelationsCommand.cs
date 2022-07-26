using Alba.CsConsoleFormat;
using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Planum.ConsoleUI
{
    public class FixTaskRelationsCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;
        ITagManager _tagManager;

        public FixTaskRelationsCommand(ITaskManager taskManager, ITagManager tagManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
            _tagManager = tagManager;
        }

        public void Execute(string command)
        {
            Serilog.Log.Information("fix task command was called");

            if (command != "fix task")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("incorrect command parameters\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            List<Task> tasks = _taskManager.GetAllTasks(null);
            foreach (Task task in tasks)
            {
                List<int> childIds = new List<int>(task.ChildIds);
                foreach (int id in task.ChildIds)
                {
                    Task? child = _taskManager.FindTask(id);
                    if (child == null)
                    {
                        childIds.Remove(id);
                    }
                }

                List<int> parentIds = new List<int>(task.ParentIds);
                foreach (int id in task.ParentIds)
                {
                    Task? parent = _taskManager.FindTask(id);
                    if (parent == null)
                    {
                        parentIds.Remove(id);
                    }
                }

                List<int> tagIds = new List<int>(task.TagIds);
                foreach (int id in task.TagIds)
                {
                    Tag? tag = _tagManager.FindTag(id);
                    if (tag == null)
                    {
                        tagIds.Remove(id);
                    }
                }

                List<int> statusIds = new List<int>(task.StatusQueueIds);
                foreach (int id in task.StatusQueueIds)
                {
                    Tag? tag = _tagManager.FindTag(id);
                    if (tag == null)
                    {
                        statusIds.Remove(id);
                    }
                }

                _taskManager.UpdateTask(new Task(task.Id, task.StartTime, task.Deadline, task.RepeatPeriod, tagIds,
                    parentIds, childIds, task.Name, task.Timed, task.UserId, task.Description, task.IsRepeated, task.Archived,
                    statusIds));
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("fixed task relations\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public string GetDescription()
        {
            return "repairs task relations, if they get messed up (missing children/tag/parents are deleted from\n" +
                "corresponding lists)";
        }

        public string GetName()
        {
            return "fix task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }

        public bool IsCommand(string command)
        {
            if (command.Split()[0] == "fix")
                return true;
            return false;
        }
    }
}
