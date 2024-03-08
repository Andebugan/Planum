using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class DelayTaskCommand : BaseCommand
    {
        TaskManager taskManager;

        TaskIdsOption idsOption;

        TimespanOption delayTimeOption = new TimespanOption("dt", "specifies delay time for task, time format: " + ArgumentParser.TimeSpanFormat, "[timespan]", TimeSpan.Zero, false);

        BoolSettingOption reverseDelayTaskOption = new BoolSettingOption("n", "reverse delay (move dates to the left)", "", false);
        BoolSettingOption startTimeOnlyOption = new BoolSettingOption("st", "delay only start time", "", false);
        BoolSettingOption deadlineOnlyOption = new BoolSettingOption("dl", "delay only deadline", "", false);

        public DelayTaskCommand(TaskManager taskManager) : base("delay", "shifts time limits of specified task/tasks by given amount of time", "[options]")
        {
            this.taskManager = taskManager;

            idsOption = new TaskIdsOption(taskManager, "i", "define task ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>(), false);
            options.Add(idsOption);
            options.Add(delayTimeOption);
            options.Add(reverseDelayTaskOption);
            options.Add(startTimeOnlyOption);   
            options.Add(deadlineOnlyOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            List<Task> tasks = taskManager.FindTask(idsOption.value);

            if (tasks.Count == 0)
            {
                ConsoleFormat.PrintWarning("tasks with the entered id do not exist");
                return;
            }

            foreach (Task task in tasks)
            {
                if (task.Start() != DateTime.MinValue && !deadlineOnlyOption.Used)
                {
                    if (reverseDelayTaskOption.Used)
                        task.TimeParams.Start = task.TimeParams.Start.Add(-delayTimeOption.value);
                    else
                        task.TimeParams.Start = task.TimeParams.Start.Add(delayTimeOption.value);
                }

                if (task.Deadline() != DateTime.MaxValue && !startTimeOnlyOption.Used)
                {
                    if (reverseDelayTaskOption.Used)
                        task.TimeParams.Deadline = task.TimeParams.Deadline.Add(-delayTimeOption.value);
                    else
                        task.TimeParams.Deadline = task.TimeParams.Deadline.Add(delayTimeOption.value);
                }

                taskManager.UpdateTask(task);
            }

            ConsoleFormat.PrintSuccess("delayed tasks succesfully");
        }
    }
}
