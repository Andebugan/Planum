using Planum.Config;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class NextRemoveOption: BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public NextRemoveOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            var next = TaskValueParser.ParseIdentity(args.Current, args.Current, TaskBufferManager.Find());
            if (!next.Any())
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find next id or name: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                {
                    var filteredDeadlines = result.DeadlineFilter.Filter(task.Deadlines);
                    foreach (var deadline in filteredDeadlines)
                        deadline.next = deadline.next.Except(next.Select(x => x.Id)).ToHashSet();
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }
            
            return true;
        }
    }
}
