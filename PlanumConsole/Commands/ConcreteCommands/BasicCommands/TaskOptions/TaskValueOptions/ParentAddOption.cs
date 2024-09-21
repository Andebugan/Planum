using Planum.Config;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class ParentAddOption : BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public ParentAddOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig)
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

            var parents = TaskValueParser.ParseIdentity(args.Current, args.Current, TaskBufferManager.Find());
            if (!parents.Any())
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find parent with id or name: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                    task.Parents = task.Parents.Concat(parents.Select(x => x.Id)).ToHashSet();
            }
            return true;
        }
    }
}
