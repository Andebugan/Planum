using System.Collections.Generic;
using System.Linq;
using Planum.Config;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Commands
{
    public class ChildRemoveOption: BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public ChildRemoveOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, CommandConfig commandConfig): base(optionInfo, commandConfig)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            var children = TaskValueParser.ParseIdentity(args.Current, args.Current, TaskBufferManager.Find());
            if (!children.Any())
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find child with id or name: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
                result.Children = result.Children.Except(children.Select(x => x.Id)).ToList();
            return true;
        }
    }
}
