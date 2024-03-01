using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.ConsoleCommands.TaskCommands;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class CopyTaskCommand : BaseCommand
    {
        TaskIdOption idOption;
        TaskNameOption nameOption;
        TaskManager taskManager;

        public CopyTaskCommand(TaskManager taskManager) : base("copy", "copies existing task", "[options]")
        {
            this.taskManager = taskManager;

            idOption = new TaskIdOption(taskManager, "i", "defines task id, from wich new task will be created", "[id_1]", -1);
            options.Add(idOption);

            nameOption = new TaskNameOption(taskManager, "n", "new name", "[name]", "");
            options.Add(nameOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            Task? copiedTask = taskManager.FindTask(idOption.value);

            if (copiedTask == null)
            {
                ConsoleFormat.PrintError("task with name: " + nameOption.value + " does not exists");
                return;
            }

            if (nameOption.Used)
            {
                if (taskManager.FindTask(nameOption.value) != null)
                {
                    ConsoleFormat.PrintError("task with name: " + nameOption.value + " already exists");
                    return;
                }
            }
            else
            {
                nameOption.value = copiedTask.Name + " new";
            }

            Task newTask = new Task(copiedTask);
            newTask.Name = nameOption.value;

            taskManager.CreateTask(newTask);

            ConsoleFormat.PrintSuccess("task copied succesfully");
        }
    }
}
