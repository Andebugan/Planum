using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Entities;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class NextRemoveOption: BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public NextRemoveOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig)
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
                if (result.CurrentDeadline is null)
                    result.CurrentDeadline = new Deadline();
                result.CurrentDeadline.next = result.CurrentDeadline.next.Except(next.Select(x => x.Id)).ToHashSet();
            }
            return true;
        }
    }
}
