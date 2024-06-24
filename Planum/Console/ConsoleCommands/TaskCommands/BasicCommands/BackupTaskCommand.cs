using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.TaskCommands
{
    public class BackupTaskCommand : BaseCommand
    {
        TaskManager taskManager;

        BoolSettingOption restoreTasks = new BoolSettingOption("r", "restore tasks from backup file", "", false);

        public BackupTaskCommand(TaskManager taskManager) : base("backup", "makes backup copy of save files", "")
        {
            this.taskManager = taskManager;

            options.Add(restoreTasks);
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            taskManager.Backup(restoreTasks.Used);
            if (restoreTasks.Used)
                ConsoleFormat.PrintSuccess("restore complete");
            else
                ConsoleFormat.PrintSuccess("backup complete");
        }
    }
}
