using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;
using System.Collections.Generic;


namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowAllArchivedTasksCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ShowAllArchivedTasksCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            List<Task> archivedTasks = _taskManager.GetAllArchivedTasks(_userManager.CurrentUser.Id);
            foreach (Task archivedTask in archivedTasks)
            {
                Console.WriteLine("Task id: " + archivedTask.Id);
                Console.WriteLine("Task name: " + archivedTask.Name);
                Console.WriteLine("Task description: " + archivedTask.Description);
                Console.Write("Task tags: ");
                foreach (int id in archivedTask.TagIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task parents: ");
                foreach (int id in archivedTask.ParentIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task children: ");
                foreach (int id in archivedTask.ChildIds)
                    Console.Write(id);
                Console.WriteLine();
                Console.WriteLine("Task is timed: " + archivedTask.Timed);
                Console.WriteLine("Task start time: " + archivedTask.StartTime.ToString());
                Console.WriteLine("Task deadline: " + archivedTask.Deadline.ToString());
                Console.WriteLine("Task is repeated: " + archivedTask.IsRepeated);
                Console.WriteLine("Task repeat period: " + archivedTask.RepeatPeriod.ToString());
                Console.WriteLine();
            }
        }

        public string GetDescription()
        {
            return "shows all archived tasks";
        }

        public string GetName()
        {
            return "show all archived tasks";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
