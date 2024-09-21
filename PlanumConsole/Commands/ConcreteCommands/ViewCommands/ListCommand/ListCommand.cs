using Planum.Config;
using Planum.Console.Commands.Selector;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Model.Managers;

namespace Planum.Console.Commands.View
{
    public class ListCommand : SelectorCommand<ListCommandSettings>
    {
        TaskBufferManager TaskBufferManager;
        RepoConfig RepoConfig;

        public ListCommand(RepoConfig repoConfig, TaskBufferManager taskBufferManager, List<SelectorBaseOption> selectorOptions, CommandInfo commandInfo, List<BaseOption<ListCommandSettings>> commandOptions, ILoggerWrapper logger) : base(selectorOptions, commandInfo, commandOptions, logger)
        {
            TaskBufferManager = taskBufferManager;
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

            TaskListWriter listWriter = new TaskListWriter(Logger, RepoConfig);
            foreach (var task in tasksToDisplay)
                listWriter.WriteTask(lines, task, tasks, listSettings);

            return lines;
        }
    }
}
