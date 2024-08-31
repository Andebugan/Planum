using System;
using System.Collections.Generic;
using System.Linq;
using Planum.Commands.Selector;
using Planum.Logger;
using Planum.Model.Managers;

namespace Planum.Commands
{
    public class CreateCommand : BaseCommand<TaskCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public CreateCommand(TaskBufferManager taskBufferManager, SelectorParser selectorParser, CommandInfo commandInfo, List<BaseOption<TaskCommandSettings>> commandOptions, ILoggerWrapper logger): base(selectorParser, commandInfo, commandOptions, logger) => TaskBufferManager = taskBufferManager;

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing create command");
            var lines = new List<string>();
            var commandSettings = new TaskCommandSettings(TaskBufferManager);
            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            var newTask = commandSettings.GetPlanumTask();
            var validationResults = TaskBufferManager.Add(newTask);
            Console.WriteLine(newTask);
            if (validationResults.Any())
                foreach (var result in validationResults)
                    lines.Add(ConsoleSpecial.AddStyle(result.Message, foregroundColor: ConsoleInfoColors.Error));

            return lines;
        }
    }
}
