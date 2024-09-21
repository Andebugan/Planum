using Planum.Config;

namespace Planum.Console.Commands.Task
{
    public class TagAddOption: BaseOption<TaskCommandSettings>
    {
        public TagAddOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            foreach (var task in result.Tasks)
                task.Tags.Add(args.Current);
            return true;
        }
    }
}
