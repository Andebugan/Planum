using Planum.Config;
using Planum.Logger;
using Planum.Model.Filters;
using Planum.Parser;

namespace Planum.Console.Commands.Selector
{
    public class SelectorDeadlineRepeatMonthsOption : SelectorBaseOption
    {
        public SelectorDeadlineRepeatMonthsOption(ILoggerWrapper logger, ConsoleConfig commandConfig, OptionInfo optionInfo) : base(logger, commandConfig, optionInfo) { }

        public override bool TryParseValue(ref IEnumerator<string> args, ref List<string> lines, ref TaskFilter result, ValueMatchType matchType, MatchFilterType matchFilterType)
        {
            if (!args.MoveNext())
            {
                lines.Add(ConsoleSpecial.AddStyle($"No arguments provided for option: {OptionInfo.Name}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            int months = 0;
            if (!ValueParser.TryParse(ref months, args.Current) && matchFilterType != MatchFilterType.SUBSTRING)
            {
                lines.Add(ConsoleSpecial.AddStyle($"Unable to parse repeat months selector option: {args.Current}", foregroundColor: ConsoleInfoColors.Error));
                return false;
            }

            IValueMatch<int> match = new ValueMatch<int>(months, args.Current);

            result.DeadlineFilter.RepeatMonthsFilter.AddMatch(match);
            return true;
        }
    }
}
