using Planum.Models.BuisnessLogic.Entities;
using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class ShowArchivedTaskCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public ShowArchivedTaskCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Console.Write("Enter task id: ");
            int taskId;
            if (!int.TryParse(Console.ReadLine(), out taskId))
            {
                Console.WriteLine("Task id must be signed integer");
                return;
            }

            Task? archivedTask = _taskManager.FindArchivedTask(taskId, _userManager.CurrentUser.Id);

            if (archivedTask == null)
            {
                Console.WriteLine("Task with specified id does not exist");
                return;
            }

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

        public string GetDescription()
        {
            return "shows archived task";
        }

        public string GetName()
        {
            return "show archived task";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
