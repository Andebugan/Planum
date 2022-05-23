using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowAllTasksCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ShowAllTasksCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            List<Task> tasks = _taskManager.GetAllTasks(_userManager.CurrentUser.Id);
            foreach (Task task in tasks)
            {
                Console.WriteLine("Task id: " + task.Id);
                Console.WriteLine("Task name: " + task.Name);
                Console.WriteLine("Task description: " + task.Description);
                Console.Write("Task tags: ");
                foreach (int id in task.TagIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task parents: ");
                foreach (int id in task.ParentIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task children: ");
                foreach (int id in task.ChildIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task is timed: " + task.Timed);
                Console.WriteLine("Task start time: " + task.StartTime.ToString());
                Console.WriteLine("Task deadline: " + task.Deadline.ToString());
                Console.WriteLine("Task is repeated: " + task.IsRepeated);
                Console.WriteLine("Task repeat period: " + task.RepeatPeriod.ToString());
                Console.WriteLine();
            }
        }

        public string GetDescription()
        {
            return "shows all tasks";
        }

        public string GetName()
        {
            return "show all tasks";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
