using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class NextAddOption: BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public NextAddOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
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
                        deadline.next = next.Select(x => x.Id).ToHashSet();
                    task.Deadlines = filteredDeadlines.ToHashSet();
                }
            }
            return true;
        }
    }
}
