using Planum.Config;
using Planum.Parser;

namespace Planum.Console.Commands.Task
{
    public class CommitOption : BaseOption<TaskCommandSettings>
    {
        public CommitOption(OptionInfo optionInfo, ConsoleConfig commandConfig): base(optionInfo, commandConfig) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskCommandSettings result)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            if (!ValueParser.TryParse(ref result.Commit, args.Current))
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse commit option value: \"{args.Current}\"", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }
            return true;
        }
    }
}
