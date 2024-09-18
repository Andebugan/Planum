using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class TagRemoveOption: BaseOption<TaskCommandSettings>
    {
        public TagRemoveOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            foreach (var task in result.Tasks)
                task.Tags.Remove(args.Current);
            return true;
        }
    }
}
