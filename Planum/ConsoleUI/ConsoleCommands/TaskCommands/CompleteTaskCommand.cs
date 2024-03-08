using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Entities;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class CompleteTaskCommand: BaseCommand
    {
        TaskIdsOption idsOption;
        BoolSettingOption uncompleteOption = new BoolSettingOption("u", "uncomplete task", "", false);

        TaskManager taskManager;

        public CompleteTaskCommand(TaskManager taskManager) : base("complete", "completes tasks with specified parameters", "[options]")
        {
            this.taskManager = taskManager;

            idsOption = new TaskIdsOption(taskManager, "i", "define task ids", "[id_1] [id_2] [id_3]-[id_m] ... [id_n]", new List<int>(), false);
            options.Add(idsOption);
            options.Add(uncompleteOption);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            foreach (var task in idsOption.value)
            {
                if (uncompleteOption.Used)
                    taskManager.UncompleteTask(task);
                else
                    taskManager.CompleteTask(task);
            }
            if (uncompleteOption.Used)
                ConsoleFormat.PrintSuccess("uncompleted succesfully");
            else
                ConsoleFormat.PrintSuccess("completed succesfully");
        }
    }
}
