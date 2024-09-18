using Planum.Config;
using Planum.Model.Managers;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class ParentRemoveOption : BaseOption<TaskCommandSettings>
    {
        protected TaskBufferManager TaskBufferManager { get; set; }

        public ParentRemoveOption(TaskBufferManager taskBufferManager, OptionInfo optionInfo, ConsoleConfig commandConfig) : base(optionInfo, commandConfig)
        {
            TaskBufferManager = taskBufferManager;
        }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            var parents = TaskValueParser.ParseIdentity(args.Current, args.Current, TaskBufferManager.Find());
            if (!parents.Any())
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to find parent with id or name: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            else
            {
                foreach (var task in result.Tasks)
                    task.Parents = task.Parents.Except(parents.Select(x => x.Id)).ToHashSet();
            }
            return true;
        }
    }
}
