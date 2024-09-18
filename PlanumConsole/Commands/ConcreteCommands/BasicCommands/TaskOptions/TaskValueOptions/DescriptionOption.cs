using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class DescriptionOption : BaseOption<TaskCommandSettings>
    {
        public DescriptionOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            foreach (var task in result.Tasks)
                task.Description = args.Current;
            return true;
        }
    }
}
