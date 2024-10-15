using Planum.Config;
using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.View
{
    public class ListCommand : SelectorCommand<ListCommandSettings>
    {
        TaskBufferManager TaskBufferManager;
        ModelConfig ModelConfig;
        RepoConfig RepoConfig;

        public ListCommand(ModelConfig modelConfig, RepoConfig repoConfig, TaskBufferManager taskBufferManager, List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<ListCommandSettings>> commandOptions, ILoggerWrapper logger) : base(selectorOptions, commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
            ModelConfig = modelConfig;
            RepoConfig = repoConfig;
        }

        public override List<string> Execute(ref IEnumerator<string> args)
        {
            Logger.Log("Executing list command");
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

            var tasks = TaskBufferManager.Find();
            Logger.Log(tasks.Count().ToString());

            var tasksToDisplay = taskFilter.Filter(tasks);
            Logger.Log(tasksToDisplay.Count().ToString());

            var listSettings = new ListCommandSettings();

            if (!ParseSettings(ref args, ref lines, ref listSettings))
                return lines;

            TaskListExporter listExporter = new TaskListExporter(ModelConfig, Logger);
            foreach (var task in tasksToDisplay)
                listExporter.WriteTask(lines, task, tasks);

            return lines;
        }
    }
}
