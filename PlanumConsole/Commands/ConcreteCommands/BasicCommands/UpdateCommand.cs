using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.Task
{
    public class UpdateCommand : SelectorCommand<TaskCommandSettings>
    {
        TaskBufferManager TaskBufferManager { get; set; }

        public UpdateCommand(TaskBufferManager taskBufferManager, List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<TaskCommandSettings>> commandOptions, ILoggerWrapper logger) : base(selectorOptions, commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing delete command");
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

            var tasksToUpdate = TaskBufferManager.Find(taskFilter).ToList();
            var commandSettings = new TaskCommandSettings(TaskBufferManager);
            commandSettings.Tasks = tasksToUpdate.ToList();
            commandSettings.DeadlineFilter = (DeadlineFilter)taskFilter.DeadlineFilter;

            if (!ParseSettings(ref args, ref lines, ref commandSettings))
                return lines;

            TaskBufferManager.Update(commandSettings.Tasks);

            lines.Add(ConsoleSpecial.AddStyle($"Updated {tasksToUpdate.Count()} tasks", foregroundColor: ConsoleInfoColors.Success));

            return lines;
        }
    }
}
