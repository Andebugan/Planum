using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class SummaryTaskCommand : BaseCommand
    {
        TaskManager taskManager;

        public SummaryTaskCommand(TaskManager taskManager) : base("summary", "shows general statistics for all tasks in the system ", "")
        {
            this.taskManager = taskManager;
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<Task> tasks = taskManager.FindTask();

            if (tasks.Count == 0)
            {
                ConsoleFormat.PrintWarning("there are no tasks in the system");
                return;
            }

            int taskCount = tasks.Count;
            int timed = 0;
            int notStarted = 0;
            int inProgress = 0;
            int overdue = 0;
            int archived = 0;
            
            foreach (var task in tasks)
            {
                if (task.Timed())
                    timed++;
                if (!task.HasStarted() && task.Timed())
                    notStarted++;
                if (task.InProgress() && task.Timed())
                    inProgress++;
                if (task.IsOverdue() && task.Timed())
                    overdue++;
                if (task.Archived)
                    archived++;
            }

            ConsoleFormat.PrintMessage("count:       ", taskCount.ToString(), ConsoleColor.DarkCyan);
            ConsoleFormat.PrintMessage("timed:       ", timed.ToString(), ConsoleColor.Cyan);
            ConsoleFormat.PrintMessage("not started: ", notStarted.ToString(), ConsoleColor.Green);
            ConsoleFormat.PrintMessage("in progress: ", inProgress.ToString(), ConsoleColor.Yellow);
            ConsoleFormat.PrintMessage("overdue:     ", overdue.ToString(), ConsoleColor.Red);
            ConsoleFormat.PrintMessage("arhived:     ", archived.ToString(), ConsoleColor.Magenta);
        }
    }
}
