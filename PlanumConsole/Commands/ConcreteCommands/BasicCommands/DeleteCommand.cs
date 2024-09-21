using System.Collections.Generic;
using System.Linq;
using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class DeleteCommand : SelectorCommand<TaskCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public DeleteCommand(TaskBufferManager taskBufferManager, List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<TaskCommandSettings>> commandOptions, ILoggerWrapper logger) : base(selectorOptions, commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing delete command");
            var lines = new List<string>();
            TaskFilter taskFilter = new TaskFilter(Logger);
            bool match = false;

            if (!ParseSelectorSettings(ref args, ref lines, ref taskFilter, ref match))
            {
                if (!match)
                {
                    Logger.Log(message: $"Unable to parse option: {args.Current}");
                    lines.Add(ConsoleSpecial.AddStyle($"Unable to parse option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                }
                return lines;
            }

            var tasksToDelete = TaskBufferManager.Find(taskFilter);
            TaskBufferManager.Delete(taskFilter);
            lines.Add(ConsoleSpecial.AddStyle($"Deleted {tasksToDelete.Count()} tasks", foregroundColor: ConsoleInfoColors.Success));

            return lines;
        }
    }
}
