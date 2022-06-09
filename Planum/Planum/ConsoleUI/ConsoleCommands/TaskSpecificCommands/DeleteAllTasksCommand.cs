using Planum.Models.BuisnessLogic.Managers;
using System;

namespace Planum.ConsoleUI.ConsoleCommands
{
    public class DeleteAllTasksCommand : ICommand
    {
        ITaskManager _taskManager;
        IUserManager _userManager;

        public DeleteAllTasksCommand(ITaskManager taskManager, IUserManager userManager)
        {
            _taskManager = taskManager;
            _userManager = userManager;
        }

        public void Execute()
        {
            Serilog.Log.Information("Delete all tasks command was called");
            _taskManager.GetAllTasks(null).ForEach(task => _taskManager.DeleteTask(task.Id));
        }

        public string GetDescription()
        {
            return "deletes all tasks";
        }

        public string GetName()
        {
            return "delete all tasks";
        }

        public bool IsAvaliable()
        {
            if (_userManager.CurrentUser != null)
                return true;
            return false;
        }
    }
}
