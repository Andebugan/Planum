using Planum.Config;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatedOption : SelectorBaseOption
    {
        public SelectorDeadlineRepeatedOption(ConsoleConfig commandConfig, OptionInfo optionInfo) : base(commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            bool enabled = true;
            if (!ValueParser.TryParse(ref enabled, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse repeated selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<bool> match = new ValueMatch<bool>(enabled, args.Current);

            result.DeadlineFilter.RepeatedFilter.AddMatch(match);
            return true;
        }
    }
}
