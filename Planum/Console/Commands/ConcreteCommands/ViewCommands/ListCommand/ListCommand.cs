using System.Collections.Generic;
using System.Linq;
using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model.Entities;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.View
{
    public class ListCommand : SelectorCommand<ListCommandSettings>
    {
        TaskBufferManager TaskBufferManager;

        public ListCommand(TaskBufferManager taskBufferManager, List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<ListCommandSettings>> commandOptions, ILoggerWrapper logger) : base(selectorOptions, commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing list command");
            var lines = new List<string>();
            TaskFilter taskFilter = new TaskFilter();
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

            var tasks = TaskBufferManager.Find();
            var tasksToDisplay = TaskBufferManager.Find(taskFilter);
            var listSettings = new ListCommandSettings();

            if (!ParseSettings(ref args, ref lines, ref listSettings))
                return lines;

            foreach (var task in tasksToDisplay)
            {
                
            }

            return lines;
        }

        void ConvertTaskToListView(PlanumTask task, ref List<string> lines)
        {

        }
    }
}
