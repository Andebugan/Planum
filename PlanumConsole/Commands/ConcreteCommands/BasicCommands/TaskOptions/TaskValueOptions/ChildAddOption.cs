using Planum.Config;
using Planum.Logger;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class ChildAddOption: BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public ChildAddOption(ILoggerWrapper logger, TaskBufferManager taskBufferManager, OptionInfo optionInfo, ConsoleConfig commandConfig): base(logger, optionInfo, commandConfig)
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

            var children = TaskValueParser.ParseIdentity(args.Current, args.Current, TaskBufferManager.Find());
            if (!children.Any())
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find child with id or name: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                    task.Children = task.Children.Concat(children).ToHashSet();
            }

            return true;
        }
    }
}
