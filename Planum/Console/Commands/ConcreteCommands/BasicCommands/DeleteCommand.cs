using System.Collections.Generic;
using System.Linq;
using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class DeleteCommand : BaseCommand<TaskCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public DeleteCommand(TaskBufferManager taskBufferManager, SelectorParser selectorParser, CommandInfo commandInfo, List<BaseOption<TaskCommandSettings>> commandOptions, ILoggerWrapper logger): base(selectorParser, commandInfo, commandOptions, logger) => TaskBufferManager = taskBufferManager;

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing delete command");
            var lines = new List<string>();

            TaskBufferManager.Delete(newTask);

            return lines;
        }
    }
}
