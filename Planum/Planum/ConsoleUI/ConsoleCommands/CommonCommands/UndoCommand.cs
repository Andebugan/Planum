using Planum.ConsoleUI.CommandProcessor;
using Planum.ConsoleUI.UI;
using Planum.Model.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Planum.ConsoleUI.ConsoleCommands.CommonCommands
{
    public class UndoCommand : BaseCommand
    {
        TaskManager taskManager;

        public UndoCommand(TaskManager taskManager) : base("undo", "undoes last action, that changed any of the tasks in any way", "")
        {
            this.taskManager = taskManager;
        }

        public override void Execute(List<string> args)
        {
            if (!ProcessOptions(ref args))
                return;

            taskManager.Undo();
            ConsoleFormat.PrintSuccess("undo complete");
        }
    }
}
